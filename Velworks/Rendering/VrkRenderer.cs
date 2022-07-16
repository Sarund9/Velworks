using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.SPIRV;
using Veldrid.StartupUtilities;
using Velworks.Collections;

namespace Velworks.Rendering;

public class VrkRenderer
{
    const int MAX_PIPELINES = 64;
    
    Sdl2Window window;
    GraphicsDevice device;
    List<IRenderPass> renderStack = new List<IRenderPass>();

    ResourceFactory factory;

    CommandList cmd;
    
    HashCache<GraphicsPipelineDescription, Pipeline> pipelineCache;

    RenderContext context = new();

    public VrkRenderer(Sdl2Window window)
    {
        this.window = window;

        device = VeldridStartup.CreateGraphicsDevice(window);
        factory = device.ResourceFactory;
        cmd = factory.CreateCommandList();

        // Create pipeline
        pipelineCache
            = new HashCache<GraphicsPipelineDescription, Pipeline>(
                factory.CreateGraphicsPipeline, MAX_PIPELINES);

    }

    public GraphicsDevice Device => device;

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

    internal void InitializeRenderPasses()
    {
        foreach (var pass in renderStack)
        {
            pass.Initialize(this);
        }
    }

    internal void Draw()
    {
        foreach (var pass in renderStack)
        {
            pass.Render(context, this);
        }
        Device.SwapBuffers();
    }
    
    #endregion

    #region RENDER PIPELINE

    public CommandList GetCommandList() =>
        cmd;

    public void AddRenderPass(IRenderPass pass)
    {
        renderStack.Add(pass);
    }

    public Pipeline GetPipeline(Mesh mesh, MaterialShader shader) =>
        pipelineCache.Get(CreatePipelineDesc(Device, mesh, shader));

    #endregion

    // TODO: Move to Utility Class
    static GraphicsPipelineDescription CreatePipelineDesc(
        GraphicsDevice device, Mesh mesh, MaterialShader matshader)
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

public class RenderContext
{

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

//public bool Match(Regex match, bool matchID = true)
//{
//    return (matchID && match.IsMatch(ID)) ||
//        GetTags().Any(x => match.IsMatch(x));
//}

