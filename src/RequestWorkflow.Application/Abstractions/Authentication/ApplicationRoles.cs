namespace RequestWorkflow.Application.Abstractions.Authentication;

public static class ApplicationRoles
{
    public const string Employee = "Employee";
    public const string Manager = "Manager";
    public const string Finance = "Finance";

    public const string Approvers = Manager + "," + Finance;
}
