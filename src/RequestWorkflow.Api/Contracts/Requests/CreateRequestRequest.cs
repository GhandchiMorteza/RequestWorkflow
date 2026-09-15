using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RequestWorkflow.Api.Contracts.Requests;

public sealed class CreateRequestRequest
{
    [Required]
    public string Title { get; init; } = string.Empty;

    [Range(
        typeof(decimal),
        "0.01",
        "9999999999999999.99")]
    public decimal Amount { get; init; }

    public string? Description { get; init; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalFields
    {
        get;
        init;
    }
}
