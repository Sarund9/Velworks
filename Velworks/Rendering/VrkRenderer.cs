namespace Velworks.Rendering;

using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.SPIRV;
using Veldrid.StartupUtilities;
using Velworks.Collections;

public class VrkRenderer
{
    const int MAX_PIPELINES = 64;
    
    Sdl2Window window;
    GraphicsDevice device;
    List<RenderPass> renderStack = new List<RenderPass>();

    ResourceFactory factory;

    CommandList cmd;
    
    HashCache<GraphicsPipelineDescription, Pipeline> pipelineCache;

    // TESTING
    Mesh testMesh;
    MaterialShader testMaterial;

    #region DEBUG

    private const string VertexCode = @"
#version 450
layout(location = 0) in vec3 Position;
layout(location = 1) in vec3 Normal;
layout(location = 2) in vec4 Color;
layout(location = 0) out vec4 v2f_Color;

void main()
{
    gl_Position = vec4(Position, 1);
    v2f_Color = Color;
}";

    private const string FragmentCode = @"
#version 450
layout(location = 0) in vec4 v2f_Color;
layout(location = 0) out vec4 drawColor;

void main()
{
    drawColor = v2f_Color;
}";
    #endregion

    public VrkRenderer(Sdl2Window window)
    {
        this.window = window;

        device = VeldridStartup.CreateGraphicsDevice(window);
        factory = device.ResourceFactory;
        cmd = factory.CreateCommandList();

        // TESTING

        var verts = new GpuVertex[]
        {
            GpuVertex.AtPos(-.75f, .75f, 0).WithColor(.9f, .1f, .1f),
            GpuVertex.AtPos(.75f, .75f, 0).WithColor(.3f, .8f, .1f),
            GpuVertex.AtPos(-.75f, -.75f, 0).WithColor(.1f, .3f, .8f),
            GpuVertex.AtPos(.75f, -.75f, 0).WithColor(0, 0, 0),
        };
        
        var indexes = new ushort[]
        {
            0, 1, 2, 3
        };

        testMesh = new Mesh(device, verts, indexes);
        testMaterial = new MaterialShader
            .Builder(device, "test")
            .Vertex(VertexCode)
            .Fragment(FragmentCode)
            //.FillMode(PolygonFillMode.Wireframe)
            .Create();

        // Create pipeline
        pipelineCache
            = new HashCache<GraphicsPipelineDescription, Pipeline>(
                factory.CreateGraphicsPipeline, MAX_PIPELINES);

    }

    /*
    TODO:
    * HashCache: test
    * Abstract Pipeline Elements
    * Abstraction: Material Shader
    * Default ECS

    * Render Graph System
    * Render Graph Asset
     */

    #region APP FLOW

    internal void InitializeRenderSystem()
    {
        foreach (var pass in renderStack)
        {
            pass.Initialize(this);
        }
    }

    internal void Draw()
    {
        cmd.Begin();
        cmd.SetFramebuffer(device.SwapchainFramebuffer);
        cmd.ClearColorTarget(0, RgbaFloat.Black);

        // TESTING
        cmd.SetVertexBuffer(0, testMesh.VertexBuffer);
        cmd.SetIndexBuffer(testMesh.IndexBuffer, testMesh.IndexFormat);

        var pipeline = pipelineCache.Get(CreatePipelineDesc(device, testMesh, testMaterial));
        cmd.SetPipeline(pipeline);

        cmd.DrawIndexed(
            indexCount: 4,
            instanceCount: 1,
            indexStart: 0,
            vertexOffset: 0,
            instanceStart: 0);

        // TEST END

        cmd.End();
        device.SubmitCommands(cmd);
        device.SwapBuffers();
    }
    
    #endregion

    #region RENDER PIPELINE

    public void AddRenderPass(RenderPass pass,
        int priority)
    {

        

    }

    #endregion

    // TODO: Move to Utility Class
    static GraphicsPipelineDescription CreatePipelineDesc(GraphicsDevice device, Mesh mesh, MaterialShader matshader)
    {
        return new GraphicsPipelineDescription
        {
            // from ??
            BlendState = BlendStateDescription.SingleOverrideBlend,
            DepthStencilState = matshader.DepthStencilState,
            RasterizerState = matshader.RasterizerState,
            // TEMP: Always the same
            PrimitiveTopology = PrimitiveTopology.TriangleStrip,
            // from ??
            ResourceLayouts = Array.Empty<ResourceLayout>(),
            ShaderSet = new ShaderSetDescription(
                vertexLayouts: new VertexLayoutDescription[] { GpuVertex.Layout() },
                shaders: matshader.ShaderPasses),
            // from ??
            Outputs = device.SwapchainFramebuffer.OutputDescription
        };
    }
}

public struct GpuVertex
{
    public Vector3 position;
    public Vector3 normal;
    public RgbaFloat color;

    public const uint SizeInBytes = sizeof(float) * (4 + 3 + 3);


    public GpuVertex WithColor(float r, float g, float b, float a = 1)
    {
        var self = this;
        self.color = new RgbaFloat(r, g, b, a);
        return self;
    }

    public static GpuVertex AtPos(float x, float y, float z) =>
        new GpuVertex
        {
            color = RgbaFloat.White,
            position = new Vector3(x, y, z),
            normal = Vector3.UnitZ,
        };
    public static GpuVertex AtPos(Vector3 pos) =>
        new GpuVertex
        {
            color = RgbaFloat.White,
            position = pos,
            normal = Vector3.UnitZ,
        };

    public static VertexLayoutDescription Layout() =>
        new VertexLayoutDescription(
            new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
            new VertexElementDescription("Normal", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
            new VertexElementDescription("Color", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4)
            );
}

public abstract class RenderPass
{
    
    protected abstract string ID { get; }
    protected abstract IEnumerable<string> GetTags();

    public bool Match(Regex match, bool matchID = true)
    {
        return (matchID && match.IsMatch(ID)) ||
            GetTags().Any(x => match.IsMatch(x));
    }


    public virtual void Initialize(VrkRenderer renderer) { }

    public abstract void Render(
        RenderContext context, VrkRenderer renderer);
}

public class RenderContext
{

}

