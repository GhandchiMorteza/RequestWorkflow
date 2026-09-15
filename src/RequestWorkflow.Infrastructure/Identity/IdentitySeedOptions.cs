namespace RequestWorkflow.Infrastructure.Identity;

public sealed class IdentitySeedOptions
{
    public const string SectionName = "IdentitySeed";

    public string EmployeePassword { get; init; } = string.Empty;

    public string ManagerPassword { get; init; } = string.Empty;

    public string FinancePassword { get; init; } = string.Empty;
}
