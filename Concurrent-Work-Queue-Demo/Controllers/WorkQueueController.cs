using System;

using Concurrent_Work_Queue;
using Microsoft.AspNetCore.Mvc;

namespace Concurrent_Work_Queue_Demo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkQueueController : ControllerBase
    {
        #region Constructors

        public WorkQueueController(ConcurrentWorkQueue<Int64> concurrentWorkQueue)
        {
            this.concurrentWorkQueue = concurrentWorkQueue;
        }

        #endregion

        #region Fields

        private readonly ConcurrentWorkQueue<Int64> concurrentWorkQueue;

        #endregion

        #region Methods

        [HttpGet]
        [Route("lease")]
        public ActionResult<ConcurrentWorkQueue<Int64>.Lease> GetLease([FromQuery(Name = "leaseTimeSeconds")] Double leaseTimeSeconds = 10.0)
        {
            if (this.concurrentWorkQueue.TryGetLease(leaseTimeSeconds, out var lease))
            {
                return this.Ok(lease);
            }

            return this.NoContent();
        }

        [HttpPost]
        [Route("enqueue")]
        public ActionResult PostEnqueue([FromBody] ConcurrentWorkQueue<Int64>.WorkOrder workOrder)
        {
            if (workOrder != null)
            {
                if (workOrder.WorkItems != null)
                {
                    foreach (var workItem in workOrder.WorkItems)
                    {
                        this.concurrentWorkQueue.Enqueue(workItem);
                    }
                }
            }

            return this.Ok();
        }

        [HttpPost]
        [Route("complete/{leaseGuid}")]
        public ActionResult PostComplete([FromRoute(Name = "leaseGuid")] Guid leaseGuid)
        {
            if (this.concurrentWorkQueue.Complete(leaseGuid))
            {
                return this.Ok();
            }

            return this.BadRequest();
        }

        #endregion
    }
}
