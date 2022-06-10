using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Velworks.Utilities
{
    public abstract unsafe class UnmanagedObject : IDisposable
    {
        Type? baseType;
        int stride;
        private bool destroyed;

        protected IntPtr Pointer { get; private set; }

        public bool Destroyed => destroyed;

        public static T Create<T>() where T : UnmanagedObject
        {
            if (Activator.CreateInstance<T>() is not T obj)
            {
                throw new ArgumentException($"Failed to create '{typeof(T)}' because it has no default constructor");
            }
            obj.baseType = typeof(T);
            obj.Init<T>();
            obj.OnCreated();
            return obj;
        }

        private void Init<T>()
        {
            stride = Marshal.SizeOf<T>();
            Pointer = Marshal.AllocHGlobal(stride);
        }

        public static bool Destroy(UnmanagedObject obj)
        {
            obj.Dispose();
            return obj.destroyed;
        }

        protected virtual void OnCreated() { }
        protected virtual void OnDestroy() { }
        protected virtual void OnDestroyUnmanaged() { }

        protected void Dispose(bool disposing)
        {
            if (!destroyed)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    OnDestroy();
                }

                OnDestroyUnmanaged();
                Marshal.FreeHGlobal(Pointer);
                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                destroyed = true;
            }
        }

        ~UnmanagedObject()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected readonly struct Field
        {
            public readonly int stride;

            
        }
        protected static Field Unmanaged<T>()
        {
            return new Field();
        }
        protected abstract IEnumerable<Field> GetFields();
        protected void GetField()
        {

        }
    }

    class Boo : UnmanagedObject
    {
        
        public static void Test()
        {

        }

        protected override IEnumerable<Field> GetFields()
        {
            yield return Unmanaged<int>();
        }
    }
}
