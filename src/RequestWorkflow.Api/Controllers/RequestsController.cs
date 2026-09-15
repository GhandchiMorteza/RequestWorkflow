using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RequestWorkflow.Api.Contracts.Requests;
using RequestWorkflow.Application.Abstractions.Authentication;
using RequestWorkflow.Application.Requests.Create;
using RequestWorkflow.Application.Requests.GetList;
using System.Text.Json;

namespace RequestWorkflow.Api.Controllers;

[ApiController]
[Route("api/requests")]
public sealed class RequestsController : ControllerBase
{
    private readonly CreateRequestService _createRequestService;
    private readonly GetRequestsService _getRequestsService;

    public RequestsController(
        CreateRequestService createRequestService,
        GetRequestsService getRequestsService)
    {
        ArgumentNullException.ThrowIfNull(createRequestService);
        ArgumentNullException.ThrowIfNull(getRequestsService);

        _createRequestService = createRequestService;
        _getRequestsService = getRequestsService;
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
}
