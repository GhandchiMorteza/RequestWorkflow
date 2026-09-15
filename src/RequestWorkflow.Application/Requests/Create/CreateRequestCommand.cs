using System.Text.Json;

namespace RequestWorkflow.Application.Requests.Create;

public sealed record CreateRequestCommand(
    string Title,
    decimal Amount,
    string? Description,
    JsonElement? Metadata);
