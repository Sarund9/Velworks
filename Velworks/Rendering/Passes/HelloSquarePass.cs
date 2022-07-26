using Veldrid;

namespace Velworks.Rendering.Passes;

public class HelloSquarePass : IRenderPass
{
    Framebuffer target;
    Mesh mesh;
    MaterialShader material;

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

    public HelloSquarePass(Framebuffer target, GraphicsDevice device)
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

        mesh = new Mesh(device, verts, indexes);
        material = new MaterialShader
            .Builder(device, "_test_pass_material_")
            .Vertex(VertexCode)
            .Fragment(FragmentCode)
            //.FillMode(PolygonFillMode.Wireframe)
            .Create();
        this.target = target;
    }

    public void Dispose()
    {

    }

    public void Render(RenderContext context, VrkRenderer renderer)
    {
        var cmd = renderer.GetCommandList();
        using (cmd.Scope())
        {
            cmd.Standart.SetFramebuffer(target);
            cmd.DrawMesh(mesh, material);
        }
        cmd.SubmitCommand();

        renderer.ReturnCommandList(cmd);
    }
}
