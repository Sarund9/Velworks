using Veldrid;

namespace Velworks.Rendering.Passes;

public sealed class ClearPass : IRenderPass
{
    RgbaFloat color;
    Framebuffer target;

    public ClearPass(Framebuffer target, RgbaFloat color)
    {
        this.color = color;
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
            cmd.Standart.ClearColorTarget(0, color);
        }
        cmd.SubmitCommand();

        renderer.ReturnCommandList(cmd);
    }


}

