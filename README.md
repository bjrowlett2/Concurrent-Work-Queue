# Concurrent-Work-Queue

A thread-safe first in-first out (FIFO) work queue.

# Concurrent-Work-Queue-Demo

To dispatch a parallel job, simply `POST` a work order to `/api/workQueue/enqueue` and invoke your Kubernetes job with
`completions: K` (where `K` is the number of items in the work order) and `parallelism: P` (where `P` is the number of
concurrent workers).

Each worker can lease a work item sending a `GET` request to `/api/workQueue/lease?leaseTimeSeconds=S` (where `S` is
the number of seconds the lease should be valid for; default is 10 seconds). A worker can signal that it is done by
making a `POST` request to `/api/workQueue/complete/{leaseGuid}`. If the lease expires before the worker can complete
the task, the work item will be placed back in the work queue.

## API Endpoints

| HTTP Method | API Endpoint                              | Success | Failure         |
|-------------|-------------------------------------------|---------|-----------------|
| GET         | /api/health                               | 200 OK  | N/A             |
| GET         | /api/workQueue/lease?leaseTimeSeconds=300 | 200 OK  | 400 Bad Request |
| POST        | /api/workQueue/enqueue                    | 200 OK  | 400 Bad Request |
| POST        | /api/workQueue/complete/{leaseGuid}       | 200 OK  | 400 Bad Request |

## Work Order (/api/workQueue/enqueue)

*In this demo the work queue just contains ints!*

```json
{
    "workItems": [
        0, 1, 2, 3, 4, 5, 6, 7, 8, 9
    ]
}
```
