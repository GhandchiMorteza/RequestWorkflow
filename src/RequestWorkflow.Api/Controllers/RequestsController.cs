using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RequestWorkflow.Api.Contracts.Requests;
using RequestWorkflow.Application.Abstractions.Authentication;
using RequestWorkflow.Application.Requests.Create;
using RequestWorkflow.Application.Requests.GetList;
using RequestWorkflow.Application.Requests.Review;
using System.Text.Json;

namespace RequestWorkflow.Api.Controllers;

[ApiController]
[Route("api/requests")]
public sealed class RequestsController : ControllerBase
{
    private readonly CreateRequestService _createRequestService;
    private readonly GetRequestsService _getRequestsService;
    private readonly ReviewRequestService _reviewRequestService;

    public RequestsController(
        CreateRequestService createRequestService,
        GetRequestsService getRequestsService,
        ReviewRequestService reviewRequestService)
    {
        ArgumentNullException.ThrowIfNull(createRequestService);
        ArgumentNullException.ThrowIfNull(getRequestsService);
        ArgumentNullException.ThrowIfNull(reviewRequestService);

        _createRequestService = createRequestService;
        _getRequestsService = getRequestsService;
        _reviewRequestService = reviewRequestService;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RequestListItem>>> GetList(
        CancellationToken cancellationToken)
    {
        var requests = await _getRequestsService.GetAsync(
            cancellationToken);

        return Ok(requests);
    }

    [Authorize(Roles = ApplicationRoles.Employee)]
    [HttpPost]
    [ProducesResponseType<CreateRequestResult>(
        StatusCodes.Status201Created)]
    public async Task<ActionResult<CreateRequestResult>> Create(
        CreateRequestRequest request,
        CancellationToken cancellationToken)
    {
        JsonElement? metadata =
            request.AdditionalFields is { Count: > 0 }
                ? JsonSerializer.SerializeToElement(
                    request.AdditionalFields)
                : null;

        var command = new CreateRequestCommand(
            request.Title,
            request.Amount,
            request.Description,
            metadata);

        var result = await _createRequestService.CreateAsync(
            command,
            cancellationToken);

        return StatusCode(
            StatusCodes.Status201Created,
            result);
    }

    [Authorize(Roles = ApplicationRoles.Approvers)]
    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(
    Guid id,
    CancellationToken cancellationToken)
    {
        var result = await _reviewRequestService.ApproveAsync(
            id,
            cancellationToken);

        return ToReviewActionResult(result);
    }

    [Authorize(Roles = ApplicationRoles.Approvers)]
    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> Reject(
    Guid id,
    CancellationToken cancellationToken)
    {
        var result = await _reviewRequestService.RejectAsync(
            id,
            cancellationToken);

        return ToReviewActionResult(result);
    }

    private IActionResult ToReviewActionResult(
    RequestReviewResult result)
    {
        return result.Outcome switch
        {
            RequestReviewOutcome.Success =>
                Ok(new
                {
                    id = result.RequestId,
                    status = result.Status
                }),

            RequestReviewOutcome.NotFound =>
                NotFound(new
                {
                    message = "Request was not found."
                }),

            RequestReviewOutcome.Forbidden =>
                StatusCode(
                    StatusCodes.Status403Forbidden,
                    new
                    {
                        message =
                            "You are not allowed to review this request."
                    }),

            RequestReviewOutcome.Conflict =>
                Conflict(new
                {
                    message =
                        "Only pending requests can be approved or rejected.",
                    status = result.Status
                }),

            _ => throw new InvalidOperationException(
                "Unknown request review outcome.")
        };
    }
}
