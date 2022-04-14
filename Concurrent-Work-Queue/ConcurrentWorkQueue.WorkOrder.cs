using System;
using System.Collections.Generic;

namespace Concurrent_Work_Queue
{
    public partial class ConcurrentWorkQueue<TWorkItem>
    {
        public class WorkOrder
        {
            #region Properties

            public List<TWorkItem> WorkItems { get; set; }

            #endregion
        }
    }
}
