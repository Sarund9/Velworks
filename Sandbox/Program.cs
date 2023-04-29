using Veldrid;
using Velworks;
using Velworks.Rendering;
using Velworks.Rendering.Passes;

VelworksApp.RunApplication<App>();

class App : VelworksApp
{

    // TODO: Unit Tests

    // TODO: Resource System
    // TODO: Standart Resource System (Basic Shaders, Basic Meshes)

    // TODO: Custom ECS

    // 

    #region BOILERPLATE

    static Mesh ExampleMesh(GraphicsDevice gd)
    {
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

        return new Mesh(gd, verts, indexes);
    }
    

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

    protected override void OnInitialize()
    {
        // Construct Render Pipeline
        Renderer.AddRenderPass(new ClearPass(
            Renderer.Device.SwapchainFramebuffer,
            RgbaFloat.Black));

        var pass = new DrawSingleMeshPass(
            Renderer.Device,
            Renderer.Device.SwapchainFramebuffer,
            ExampleMesh(Renderer.Device),
            new MaterialShader
                .Builder(Renderer.Device, "fill")
                .Vertex(VertexCode)
                .Fragment(FragmentCode)
                .Create());

        Renderer.AddRenderPass(pass);





    }
}

