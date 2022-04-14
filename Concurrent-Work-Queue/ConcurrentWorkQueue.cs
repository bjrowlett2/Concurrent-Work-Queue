using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Concurrent_Work_Queue
{
    public partial class ConcurrentWorkQueue<TWorkItem> : IDisposable
    {
        #region Constructors

        public ConcurrentWorkQueue()
        {
            this.cancellationTokenSource = new CancellationTokenSource();
            this.autoReconciliationThread = new Thread(this.AutoReconciliationLoop);
            this.autoReconciliationThread.Start();
        }

        #endregion

        #region Finalizer

        ~ConcurrentWorkQueue()
        {
            this.Dispose(false);
            this.cancellationTokenSource.Cancel();
        }

        #endregion

        #region Fields

        private readonly Object mutex = new();

        private readonly ConcurrentQueue<TWorkItem> queue = new();

        private readonly ConcurrentDictionary<Guid, Lease> leases = new();

        private CancellationTokenSource cancellationTokenSource;

        private readonly Thread autoReconciliationThread;

        #endregion

        #region Properties

        public Int32 Count
        {
            get
            {
                lock (this.mutex)
                {
                    return this.queue.Count + this.leases.Count;
                }
            }
        }

        #endregion

        #region Methods

        public void Enqueue(TWorkItem value)
        {
            lock (this.mutex)
            {
                this.queue.Enqueue(value);
            }
        }

        public Boolean Complete(Guid leaseKey)
        {
            try
            {
                lock (this.mutex)
                {
                    if (this.leases.TryRemove(leaseKey, out var _))
                    {
                        return true; // The work item is no longer tracked by the work queue.
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine($"[WARN] An exception was thrown while trying to complete a work item.");
            }

            return false;
        }

        public Boolean TryGetLease(Double leaseTimeInSeconds, out Lease lease)
        {
            lease = null;

            lock (this.mutex)
            {
                if (this.queue.TryDequeue(out var workItem))
                {
                    try
                    {
                        lease = new Lease()
                        {
                            Key = Guid.NewGuid(), // @Incomplete: Guarentee uniqueness.
                            WorkItem = workItem,
                            ExpirationDate = DateTime.Now.AddSeconds(leaseTimeInSeconds),
                        };

                        this.leases.GetOrAdd(lease.Key, lease);

                        return true;
                    }
                    catch (Exception)
                    {
                        this.queue.Enqueue(workItem);

                        Console.WriteLine("[WARN] An exception was thrown while trying to lease a work item; work item will be placed back in the work queue.");
                    }
                }
            }

            return false;
        }

        private void AutoReconciliationLoop()
        {
            while (true)
            {
                Thread.Sleep(250);

                if (this.cancellationTokenSource.IsCancellationRequested)
                {
                    break;
                }

                var now = DateTime.Now;
                foreach (var leaseKey in this.leases.Keys)
                {
                    try
                    {
                        if (this.leases.TryGetValue(leaseKey, out var lease))
                        {
                            if (lease.ExpirationDate < now)
                            {
                                lock (this.mutex)
                                {
                                    if (this.leases.TryRemove(leaseKey, out var _))
                                    {
                                        this.queue.Enqueue(lease.WorkItem);

                                        Console.WriteLine($"[INFO] The lease on '{leaseKey}' has expired; work item will be re-queued.");
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("[WARN] An exception was thrown while trying to reconcile a lease; nothing will be done.");
                    }
                }
            }
        }

        #endregion

        #region IDisposable Support

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                if (this.cancellationTokenSource != null)
                {
                    this.cancellationTokenSource.Cancel();
                    this.cancellationTokenSource.Dispose();
                }
            }
        }

        #endregion
    }
}
