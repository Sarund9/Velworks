using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Velworks.Collections;

public class Pool<T>
{
    private readonly ConcurrentBag <T> bag = new();
    private readonly Func<T> getNew;

    public Pool(Func<T> getNew, int initialAmmount = 0)
    {
        this.getNew = getNew;

        for (int i = 0; i < initialAmmount; i++)
        {
            bag.Add(getNew());
        }
    }

    public bool IsEmpty => bag.IsEmpty;
    public int CachedCount => bag.Count;
    public IEnumerable<T> Uninstantiated => bag;

    public T Get()
    {
        if (bag.TryTake(out var result))
            return result;
        
        return getNew();
    }

    public void Return(T item) => bag.Add(item);

    public void Clear() => bag.Clear();
    public bool IsCached(T item) => bag.Contains(item);

}
