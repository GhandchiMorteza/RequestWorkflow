using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace RequestWorkflow.Api.OpenApi;

public sealed class AuthOperationTransformer
    : IOpenApiOperationTransformer
{
    public Task TransformAsync(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        var metadata =
            context.Description
                .ActionDescriptor
                .EndpointMetadata;

        var allowsAnonymous =
            metadata.OfType<IAllowAnonymous>().Any();

        var requiresAuthorization =
            metadata.OfType<IAuthorizeData>().Any();

        if (allowsAnonymous || !requiresAuthorization)
        {
            return Task.CompletedTask;
        }

        operation.Security ??= [];

        if (context.Document is not null)
        {
            operation.Security.Add(
                new OpenApiSecurityRequirement
                {
                    [
                        new OpenApiSecuritySchemeReference(
                            "Bearer",
                            context.Document)
                    ] = []
                });
        }

        return Task.CompletedTask;
    }
}
