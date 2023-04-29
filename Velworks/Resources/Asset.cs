using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Velworks.Resources;


public abstract class Asset : IDisposable
{
    protected int RefCount;

    readonly List<IDisposable> subdisposables = new();

    protected void AddDisposable()
    {
        subdisposables.Add(this);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        subdisposables.ForEach(x => x.Dispose());
        OnDisposed();
    }

    /// <summary> Called when object is Disposed </summary>
    protected virtual void OnDisposed() { }
}

public abstract class Asset<K, A> : Asset
    where K : notnull
    where A : Asset
{

    readonly static Manager manager = new();

    protected static bool TryGetAsset(K key, out A? asset)
    {
        if (manager.Try(key, out asset))
        {
            var a = asset as Asset<K, A>;
            a?.IncrementCount();
            return true;
        }
        return false;
    }

    private void IncrementCount()
    {
        RefCount++;
    }

    public void Return()
    {
        
    }

    class Manager
    {
        readonly Dictionary<K, A> assets = new();

        public bool Try(K key, out A? asset) => assets.TryGetValue(key, out asset);
    }
}

public class Mesh : Asset<string, Mesh>
{

    public static Mesh Load(string assetpath)
    {
        return null;
    }
}

/// <summary>
/// Reference a Mesh inside a file
/// Could be one of many meshes inside an FBX, OBJ, etc...
/// </summary>
public struct FileMesh
{
    public string path;
    
}
