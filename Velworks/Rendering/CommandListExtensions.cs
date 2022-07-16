using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace Velworks.Rendering;

public static class CommandListExtensions
{

    public static void DrawMesh(this CommandList cmd,
        Mesh mesh, MaterialShader shader)
    {
        var renderer = VelworksApp.Instance.Renderer;

        var pipeline = renderer.GetPipeline(mesh, shader);
        cmd.SetPipeline(pipeline);

        cmd.SetVertexBuffer(0, mesh.VertexBuffer);
        cmd.SetIndexBuffer(mesh.IndexBuffer, mesh.IndexFormat);

        cmd.DrawIndexed(
            indexCount: mesh.IndexCount,
            instanceCount: 1,
            indexStart: 0,
            vertexOffset: 0,
            instanceStart: 0);
    }

}
