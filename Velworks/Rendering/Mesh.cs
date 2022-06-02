namespace Velworks.Rendering;

using System.Numerics;
using Veldrid;

public class Mesh : IDisposable
{
    GpuVertex[]? localVertices;
    ushort[]? localIndexes;

    GraphicsDevice gd;

    public Mesh(GraphicsDevice gd, GpuVertex[] vertices, ushort[] indexes)
    {
        var vertDesc = new BufferDescription(
            GpuVertex.SizeInBytes * (uint)vertices.Length,
            BufferUsage.VertexBuffer);
        var indDesc = new BufferDescription(
            sizeof(ushort) * (uint)indexes.Length,
            BufferUsage.IndexBuffer);
        
        VertexBuffer = gd.ResourceFactory.CreateBuffer(ref vertDesc);
        gd.UpdateBuffer(VertexBuffer, 0, vertices);
        IndexBuffer = gd.ResourceFactory.CreateBuffer(ref indDesc);
        gd.UpdateBuffer(IndexBuffer, 0, indexes);
        this.gd = gd;
    }
    
    public DeviceBuffer VertexBuffer { get; private set; }
    public DeviceBuffer IndexBuffer { get; private set; }

    public IndexFormat IndexFormat => IndexFormat.UInt16;

    public void SetLocalVertexData(GpuVertex[] data)
    {
        localVertices = data;
    }
    public void SetLocalIndexData(ushort[] data)
    {
        localIndexes = data;
    }

    public void Apply()
    {
        if (localVertices != null)
        {
            VertexBuffer.Dispose();
            var vertDesc = new BufferDescription(
                GpuVertex.SizeInBytes * (uint)localVertices.Length,
                BufferUsage.VertexBuffer);
            VertexBuffer = gd.ResourceFactory.CreateBuffer(ref vertDesc);
            gd.UpdateBuffer(VertexBuffer, 0, localVertices);
        }
        if (localIndexes != null)
        {
            IndexBuffer.Dispose();
            var indDesc = new BufferDescription(
                sizeof(ushort) * (uint)localIndexes.Length,
                BufferUsage.IndexBuffer);
            IndexBuffer = gd.ResourceFactory.CreateBuffer(ref indDesc);
            gd.UpdateBuffer(IndexBuffer, 0, localIndexes);
        }
    }

    public bool RegenerateNormals()
    {
        if (localIndexes is null || localVertices is null)
        {
            return false;
        }

        var polyCount = new byte[localVertices.Length];

        static Vector3 GenNewNormal(ref Vector3 p1, ref Vector3 p2, ref Vector3 p3)
        {
            var l1 = p2 - p1;
            var l2 = p3 - p1;
            return Vector3.Cross(l1, l2);
        }

        for (int i = 0; i < localIndexes.Length; i++)
        {
            ref var index = ref localIndexes[i];
            
            // Create new Normal
            var mod = i % 3;
            var normal = GenNewNormal(
                ref localVertices[index - mod].position,
                ref localVertices[index - mod + 1].position,
                ref localVertices[index - mod + 2].position
                );

            if (polyCount[index] == 0)
            {
                localVertices[index].normal = normal;
            }
            else
            {
                localVertices[index].normal = Vector3.Lerp(localVertices[index].normal, normal, .5f / polyCount[index]);
            }
            polyCount[index]++;
        }

        return true;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        VertexBuffer.Dispose();
        IndexBuffer.Dispose();
    }


}
