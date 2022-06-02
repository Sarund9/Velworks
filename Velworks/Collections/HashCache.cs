using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Velworks.Collections
{
    public class HashCache<TKey, TResult>
        where TKey : notnull
    {

        Dictionary<TKey, TResult> cache = new Dictionary<TKey, TResult>();
        Func<TKey, TResult> get;
        
        ValueMemento<TKey> values;

        public HashCache(Func<TKey, TResult> get, int maxCap)
        {
            if (maxCap < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maxCap) + " should be greater than 0");
            }

            this.get = get;
            values = new ValueMemento<TKey>(maxCap);
        }

        public TResult Get(TKey key)
        {
            if (cache.ContainsKey(key))
            {
                return cache[key];
            }
            else
            {
                var item = get(key);
                cache.Add(key, item);

                TKey? old = values.AddTryRemove(key);
                if (old != null)
                {
                    cache.Remove(old);
                }
                return item;
            }
        }


    }

    public class ValueMemento<T>
    {
        Queue<T> queue = new Queue<T>();
        int capacity;

        public ValueMemento(int capacity)
        {
            if (capacity < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(capacity) + " should be greater than 0");
            }

        }

        public T? AddTryRemove(T key)
        {
            queue.Enqueue(key);
            if (queue.Count < capacity)
            {
                return default(T);
            }
            return queue.Dequeue();
        }
    }
}
