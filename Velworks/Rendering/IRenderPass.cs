namespace Velworks.Rendering;

public interface IRenderPass
{
    public void Initialize(VrkRenderer renderer) { }

    public void Render(
        RenderContext context, VrkRenderer renderer);
}

