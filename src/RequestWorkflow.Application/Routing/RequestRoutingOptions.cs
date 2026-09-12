namespace RequestWorkflow.Application.Requests.Routing;

public sealed class RequestRoutingOptions
{
    public const string SectionName = "RequestRouting";

    public decimal ManagerApprovalMaxAmount { get; init; }
}
