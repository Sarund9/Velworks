namespace Velworks.Rendering;

using System.Numerics;
using System.Text;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.SPIRV;
using Veldrid.StartupUtilities;

public class VrkRenderer
{
    Sdl2Window window;
    GraphicsDevice device;
    List<RenderPass> renderStack = new List<RenderPass>();

    ResourceFactory factory;

    CommandList cmd;
    Pipeline pipeline;

    // TESTING
    Mesh testMesh;
    
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

        // Create Vertices

        var shDescV = ShaderFrom(VertexCode, ShaderStages.Vertex);
        var shDescF = ShaderFrom(FragmentCode, ShaderStages.Fragment);
        var shaders = factory.CreateFromSpirv(shDescV, shDescF);
        
        // Create pipeline
        GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription
        {
            // from ??
            BlendState = BlendStateDescription.SingleOverrideBlend,
            // from MaterialShader
            DepthStencilState = new DepthStencilStateDescription(
                depthTestEnabled: true,
                depthWriteEnabled: true,
                comparisonKind: ComparisonKind.LessEqual),
            // from MaterialShader
            RasterizerState = new RasterizerStateDescription(
                cullMode: FaceCullMode.Back,
                fillMode: PolygonFillMode.Solid,
                frontFace: FrontFace.Clockwise,
                depthClipEnabled: true,
                scissorTestEnabled: false),
            // TEMP: Always the same
            PrimitiveTopology = PrimitiveTopology.TriangleStrip,
            // from ??
            ResourceLayouts = Array.Empty<ResourceLayout>(),
            // from ??
            ShaderSet = new ShaderSetDescription(
                vertexLayouts: new VertexLayoutDescription[] { GpuVertex.Layout() },
                shaders: shaders),
            // from ??
            Outputs = device.SwapchainFramebuffer.OutputDescription
        };

        pipeline = factory.CreateGraphicsPipeline(pipelineDescription);
    }

    /*
    TODO:
    * Source Control
    * HashCache: test
    * Abstract Pipeline Elements
    * Abstraction: Material Shader
    * Shader System
    * Asset Manager
    * Shader Loader
    * 
    * Render Graph System
    * Render Graph Asset
     */
    internal void Draw()
    {
        cmd.Begin();
        cmd.SetFramebuffer(device.SwapchainFramebuffer);
        cmd.ClearColorTarget(0, RgbaFloat.Black);

        // TESTING
        cmd.SetVertexBuffer(0, testMesh.VertexBuffer);
        cmd.SetIndexBuffer(testMesh.IndexBuffer, testMesh.IndexFormat);
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
    static ShaderDescription ShaderFrom(string src, ShaderStages stage) =>
            new ShaderDescription(
                stage,
                Encoding.UTF8.GetBytes(src),
                "main");
    
}

public class MaterialShader
{

    public MaterialShader(GraphicsDevice gd, params ShaderDescription[] passes)
    {
        var fac = gd.ResourceFactory;

        ShaderPasses = new Shader[passes.Length];

        for (int i = 0; i < passes.Length; i++)
        {
            ShaderPasses[i] = fac.CreateShader(ref passes[i]);
        }
    }

    public Shader[] ShaderPasses { get; private set; }

    //public struct Pass
    //{
    //    public ShaderDescription desc;

    //    public static implicit operator Pass(
    //        (string src, ShaderStages stage) data) =>
    //        new Pass { desc = new ShaderDescription(
    //            data.stage,
    //            Encoding.UTF8.GetBytes(data.src),
    //            "main") };
    //    public static implicit operator ShaderDescription(Pass pass) =>
    //        pass.desc;
    //}
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

}


