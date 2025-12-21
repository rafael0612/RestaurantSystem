using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace RestaurantSystem.API.OpenApi
{
    public sealed class BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
    {
        public async Task TransformAsync(
            OpenApiDocument document,
            OpenApiDocumentTransformerContext context,
            CancellationToken cancellationToken)
        {
            // Solo agrega si existe el esquema Bearer registrado en Authentication
            var schemes = await authenticationSchemeProvider.GetAllSchemesAsync();
            if (!schemes.Any(s => s.Name == "Bearer"))
                return;

            // 1) Registrar el security scheme a nivel documento
            var securitySchemes = new Dictionary<string, IOpenApiSecurityScheme>
            {
                ["Bearer"] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    BearerFormat = "JWT",
                    Description = "JWT Authorization header. Ej: \"Bearer {token}\""
                }
            };

            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes = securitySchemes;

            // 2) Aplicarlo como requirement en todas las operaciones
            foreach (var operation in document.Paths.Values.SelectMany(path => path.Operations!))
            {
                operation.Value.Security ??= [];
                operation.Value.Security.Add(new OpenApiSecurityRequirement
                {
                    // clave: referencia al scheme "Bearer"
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                });
            }
        }
    }
}
