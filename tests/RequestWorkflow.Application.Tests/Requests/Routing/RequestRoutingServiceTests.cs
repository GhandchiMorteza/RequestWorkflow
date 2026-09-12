using RequestWorkflow.Application.Requests.Routing;
using RequestWorkflow.Domain.Requests;

namespace RequestWorkflow.Application.Tests.Requests.Routing;

public sealed class RequestRoutingServiceTests
{
    private readonly RequestRoutingService _sut;

    public RequestRoutingServiceTests()
    {
        var options = new RequestRoutingOptions
        {
            ManagerApprovalMaxAmount = 1_000_000
        };

        _sut = new RequestRoutingService(options);
    }

    [Fact]
    public void DetermineApprovalRole_WhenAmountIsBelowThreshold_ReturnsManager()
    {
        var result = _sut.DetermineApprovalRole(999_999);

        Assert.Equal(ApprovalRole.Manager, result);
    }

    [Fact]
    public void DetermineApprovalRole_WhenAmountEqualsThreshold_ReturnsManager()
    {
        var result = _sut.DetermineApprovalRole(1_000_000);

        Assert.Equal(ApprovalRole.Manager, result);
    }

    [Fact]
    public void DetermineApprovalRole_WhenAmountIsAboveThreshold_ReturnsFinance()
    {
        var result = _sut.DetermineApprovalRole(1_000_001);

        Assert.Equal(ApprovalRole.Finance, result);
    }
}
