﻿using Veldrid;

namespace Velworks.Rendering.Passes;

public sealed class DrawSingleMeshPass : IRenderPass
{
    Framebuffer target;
    Mesh mesh;
    MaterialShader material;

    public DrawSingleMeshPass(
        GraphicsDevice gd, Framebuffer target,
        Mesh mesh, MaterialShader material)
    {
        this.target = target;
        this.mesh = mesh;
        this.material = material;
    }

    public void Dispose()
    {
        mesh.Dispose();
        material.Dispose();
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
