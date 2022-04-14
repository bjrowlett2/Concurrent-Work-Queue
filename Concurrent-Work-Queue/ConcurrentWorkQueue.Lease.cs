using System;

namespace Concurrent_Work_Queue
{
    public partial class ConcurrentWorkQueue<TWorkItem>
    {
        public class Lease
        {
            #region Properties

            public Guid Key { get; init; }

            public TWorkItem WorkItem { get; init; }

            public DateTime ExpirationDate { get; init; }

            #endregion
        }
    }
}
