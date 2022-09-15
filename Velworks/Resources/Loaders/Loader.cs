using DefaultEcs;
using DefaultEcs.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Velworks.Rendering;

namespace Velworks.Resources.Loaders;

[Serializable]
public struct MeshInfo
{
    GpuVertex[] vertices;
    uint[] indices;

    public void Des()
    {

    }
}

public class MeshLoader : AResourceManager<string, Mesh>
{
    protected override Mesh Load(string info)
    {
        throw new NotImplementedException();
    }

    protected override void OnResourceLoaded(in Entity entity, string info, Mesh resource)
    {
        
    }
}

