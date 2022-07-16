using Veldrid;

namespace Velworks.Rendering.Passes;

public class ClearPass : IRenderPass
{
    RgbaFloat color;

    public ClearPass(RgbaFloat color)
    {
        this.color = color;
    }

    public void Render(RenderContext context, VrkRenderer renderer)
    {
        var cmd = renderer.GetCommandList();
        cmd.Begin();
        cmd.SetFramebuffer(renderer.Device.SwapchainFramebuffer);

        cmd.ClearColorTarget(0, color);

        cmd.End();
        renderer.Device.SubmitCommands(cmd);
    }
}

