using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Firmaro.Api.Extensions
{
    public sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
    {
        private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;

        public BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider)
        {
            _authenticationSchemeProvider = authenticationSchemeProvider;
        }

        public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
        {
            if (document.Paths is null)
                return;

            IEnumerable<AuthenticationScheme> authenticationSchemes = await _authenticationSchemeProvider.GetAllSchemesAsync();
            if (!authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
                return;

            OpenApiSecurityScheme bearerScheme = new()
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                In = ParameterLocation.Header,
                BearerFormat = "Json Web Token"
            };

            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
            document.Components.SecuritySchemes["Bearer"] = bearerScheme;

            OpenApiSecurityRequirement requirement = new() { [new OpenApiSecuritySchemeReference("Bearer")] = [] };

            foreach (IOpenApiPathItem path in document.Paths.Values)
            {
                if (path.Operations is null)
                    continue;

                foreach (OpenApiOperation operation in path.Operations.Values)
                {
                    operation.Security ??= [];
                    operation.Security.Add(requirement);
                }
            }
        }
    }
}