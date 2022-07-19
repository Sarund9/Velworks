namespace Velworks.Rendering;

public interface IRenderPass : IDisposable
{
    public void Initialize(VrkRenderer renderer) { }

    public void Render(
        RenderContext context, VrkRenderer renderer);
}

