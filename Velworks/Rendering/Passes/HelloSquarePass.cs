using Veldrid;

namespace Velworks.Rendering.Passes;

public class HelloSquarePass : IRenderPass
{
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

    public HelloSquarePass(GraphicsDevice device)
    {
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

    }

    public void Render(RenderContext context, VrkRenderer renderer)
    {
        var cmd = renderer.GetCommandList();

        cmd.Begin();
        cmd.SetFramebuffer(renderer.Device.SwapchainFramebuffer);

        // TESTING
        cmd.DrawMesh(testMesh, testMaterial);

        // TEST END
        cmd.End();
        renderer.Device.SubmitCommands(cmd);
    }
}

