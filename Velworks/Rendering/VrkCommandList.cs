using Veldrid;

namespace Velworks.Rendering;

public struct VrkCommandList
{
    public VrkCommandList(VrkRenderer renderer, CommandList cmd)
    {
        this.Renderer = renderer;
        this.Standart = cmd;
    }
    public VrkRenderer Renderer { get; }
    public CommandList Standart { get; }

    /// <summary>
    /// Sets a Pipeline, VertexBuffer and Indexbuffer.
    /// Then calls DrawIndexed.
    /// </summary>
    public void DrawMesh(Mesh mesh, MaterialShader shader)
    {
        var pipeline = Renderer.GetPipeline(mesh, shader);
        Standart.SetPipeline(pipeline);

        Standart.SetVertexBuffer(0, mesh.VertexBuffer);
        Standart.SetIndexBuffer(mesh.IndexBuffer, mesh.IndexFormat);

        Standart.DrawIndexed(
            indexCount: mesh.IndexCount,
            instanceCount: 1,
            indexStart: 0,
            vertexOffset: 0,
            instanceStart: 0);
    }

    /// <summary> Submits commands to the Renderer's Graphic Device </summary>
    public void SubmitCommand()
    {
        Renderer.Device.SubmitCommands(Standart);
    }


    public ScopeValue Scope()
    {
        Standart.Begin();
        return new ScopeValue(this);
    }

    public struct ScopeValue : IDisposable
    {
        VrkCommandList cl;
        public ScopeValue(VrkCommandList cl) => this.cl = cl;
        public void Dispose() => cl.Standart.End();
    }
}
