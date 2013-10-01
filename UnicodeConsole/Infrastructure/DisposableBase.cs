using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnicodeConsole.Infrastructure
{
    public abstract class DisposableBase : IDisposable
    {
        private bool _disposed;

        //Implement IDisposable.
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        protected bool IsDisposed { get { return _disposed; } }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                // Free other state (managed objects).
                DisposeManagedMembers();
            }
            _disposed = true;
            // Free your own state (unmanaged objects).
            DisposeUnmanagedMembers();
            // Set large fields to null.
        }

        protected abstract void DisposeUnmanagedMembers();

        protected abstract void DisposeManagedMembers();

        // Use C# destructor syntax for finalization code.
        ~DisposableBase()
        {
            // Simply call Dispose(false).
            Dispose(false);
        }
    }
}
