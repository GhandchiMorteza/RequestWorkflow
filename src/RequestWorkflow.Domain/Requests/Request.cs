using System.Text.Json;

namespace RequestWorkflow.Domain.Requests;

public sealed class Request
{
    private Request()
    {
    }

    private Request(
        Guid id,
        string title,
        decimal amount,
        string? description,
        Guid createdByUserId,
        ApprovalRole assignedRole,
        DateTimeOffset createdAt,
        JsonElement? metadata)
    {
        Id = id;
        Title = title;
        Amount = amount;
        Description = description;
        CreatedByUserId = createdByUserId;
        AssignedRole = assignedRole;
        CreatedAt = createdAt;
        Metadata = metadata?.Clone();
        Status = RequestStatus.Pending;
    }

    public Guid Id { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public decimal Amount { get; private set; }

    public string? Description { get; private set; }

    public RequestStatus Status { get; private set; }

    public Guid CreatedByUserId { get; private set; }

    public ApprovalRole AssignedRole { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public JsonElement? Metadata { get; private set; }

    public static Request Create(
        string title,
        decimal amount,
        string? description,
        Guid createdByUserId,
        ApprovalRole assignedRole,
        DateTimeOffset createdAt,
        JsonElement? metadata = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "Amount must be greater than zero.");
        }

        if (createdByUserId == Guid.Empty)
        {
            throw new ArgumentException(
                "Created by user id is required.",
                nameof(createdByUserId));
        }

        if (!Enum.IsDefined(assignedRole))
        {
            throw new ArgumentOutOfRangeException(
                nameof(assignedRole),
                "Assigned role is invalid.");
        }

        return new Request(
            Guid.NewGuid(),
            title.Trim(),
            amount,
            string.IsNullOrWhiteSpace(description)
                ? null
                : description.Trim(),
            createdByUserId,
            assignedRole,
            createdAt,
            metadata);
    }

    public void Approve()
    {
        EnsurePending();
        Status = RequestStatus.Approved;
    }

    public void Reject()
    {
        EnsurePending();
        Status = RequestStatus.Rejected;
    }

    private void EnsurePending()
    {
        if (Status != RequestStatus.Pending)
        {
            throw new InvalidOperationException(
                "Only pending requests can be approved or rejected.");
        }
    }
}
