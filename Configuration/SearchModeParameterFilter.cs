using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DotNetMovieApi.Configuration;

public sealed class SearchModeParameterFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (!string.Equals(context.ApiDescription.HttpMethod, HttpMethods.Get, StringComparison.OrdinalIgnoreCase) ||
            operation.Parameters is null)
        {
            return;
        }

        var lowerCaseParameter = operation.Parameters.FirstOrDefault(parameter =>
            string.Equals(parameter.Name, "searchMode", StringComparison.Ordinal));
        var upperCaseParameter = operation.Parameters.FirstOrDefault(parameter =>
            string.Equals(parameter.Name, "SearchMode", StringComparison.Ordinal));

        if (lowerCaseParameter is not null && upperCaseParameter is not null)
        {
            operation.Parameters.Remove(upperCaseParameter);
            return;
        }

        if (upperCaseParameter is null)
        {
            return;
        }

        var upperCaseParameterIndex = operation.Parameters.IndexOf(upperCaseParameter);
        if (upperCaseParameterIndex < 0)
        {
            return;
        }

        operation.Parameters[upperCaseParameterIndex] = new OpenApiParameter
        {
            Name = "searchMode",
            In = upperCaseParameter.In,
            Description = upperCaseParameter.Description,
            Required = upperCaseParameter.Required,
            Deprecated = upperCaseParameter.Deprecated,
            AllowEmptyValue = upperCaseParameter.AllowEmptyValue,
            Style = upperCaseParameter.Style,
            Explode = upperCaseParameter.Explode,
            AllowReserved = upperCaseParameter.AllowReserved,
            Schema = upperCaseParameter.Schema,
            Examples = upperCaseParameter.Examples,
            Example = upperCaseParameter.Example,
            Content = upperCaseParameter.Content,
            Extensions = upperCaseParameter.Extensions
        };
    }
}
