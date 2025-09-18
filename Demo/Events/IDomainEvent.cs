public interface IDomainEvent { }

// Example usage:
// 
// await _eventDispatcher.DispatchAsync(
//     new JobAppliedEvent(jobId, jobseekerId, employerId)
// );