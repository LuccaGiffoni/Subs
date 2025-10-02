namespace Subs.Domain.Responses;

public class WorkerResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid? SubscriptionId { get; set; }
    public List<string> Errors { get; set; } = [];

    public WorkerResponse() { }

    public WorkerResponse(bool success, string message, Guid? subscriptionId = null, List<string>? errors = null)
    {
        Success = success;
        Message = message;
        SubscriptionId = subscriptionId;
        Errors = errors ?? [];
    }
}