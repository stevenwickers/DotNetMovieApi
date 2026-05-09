using System.Text.Json.Nodes;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DotNetMovieApi.Configuration;

public sealed class GraphQlSwaggerDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.Paths ??= new OpenApiPaths();
        swaggerDoc.Tags ??= new HashSet<OpenApiTag>();

        if (!swaggerDoc.Tags.Any(tag => string.Equals(tag.Name, "GraphQL", StringComparison.Ordinal)))
        {
            swaggerDoc.Tags.Add(new OpenApiTag
            {
                Name = "GraphQL",
                Description = "Hot Chocolate GraphQL endpoint."
            });
        }

        var pathItem = new OpenApiPathItem
        {
            Summary = "GraphQL endpoint",
            Description = "Executes GraphQL queries and mutations through Hot Chocolate."
        };

        pathItem.AddOperation(HttpMethod.Post, new OpenApiOperation
        {
            Summary = "Execute GraphQL query or mutation",
            Description =
                """
                Use this endpoint to run the Hot Chocolate GraphQL operations exposed by the API.

                Available operations include:
                - `movies(filters: MovieFiltersInput)`
                - `movieById(id: UUID!)`
                - `updateMovie(id: UUID!, patch: MoviePatchRequestInput!)`
                """,
            OperationId = "ExecuteGraphQl",
            Tags = new HashSet<OpenApiTagReference>
            {
                new("GraphQL", swaggerDoc, null)
            },
            RequestBody = new OpenApiRequestBody
            {
                Required = true,
                Description = "Standard GraphQL HTTP request body.",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new()
                    {
                        Schema = CreateGraphQlRequestSchema(),
                        Examples = CreateRequestExamples()
                    }
                }
            },
            Responses = new OpenApiResponses
            {
                ["200"] = new OpenApiResponse
                {
                    Description = "GraphQL response envelope. Field shape depends on the submitted query or mutation.",
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new()
                        {
                            Schema = CreateGraphQlResponseSchema(),
                            Example = JsonNode.Parse(
                                """
                                {
                                  "data": {
                                    "movieById": {
                                      "id": "00000000-0000-0000-0000-000000000000",
                                      "movieName": "Avatar"
                                    }
                                  }
                                }
                                """)
                        }
                    }
                },
                ["400"] = new OpenApiResponse
                {
                    Description = "The GraphQL request could not be parsed or validated."
                }
            }
        });

        swaggerDoc.Paths["/graphql"] = pathItem;
    }

    private static OpenApiSchema CreateGraphQlRequestSchema()
    {
        return new OpenApiSchema
        {
            Type = JsonSchemaType.Object,
            Required = new HashSet<string> { "query" },
            Properties = new Dictionary<string, IOpenApiSchema>
            {
                ["query"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Description = "GraphQL query or mutation document."
                },
                ["variables"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.Object,
                    Description = "Optional variables referenced by the query document.",
                    AdditionalPropertiesAllowed = true
                },
                ["operationName"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Description = "Optional operation name when the document contains multiple operations."
                }
            }
        };
    }

    private static OpenApiSchema CreateGraphQlResponseSchema()
    {
        return new OpenApiSchema
        {
            Type = JsonSchemaType.Object,
            Properties = new Dictionary<string, IOpenApiSchema>
            {
                ["data"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.Object,
                    Description = "Result data for the selected GraphQL operation.",
                    AdditionalPropertiesAllowed = true
                },
                ["errors"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.Array,
                    Description = "GraphQL execution or validation errors.",
                    Items = new OpenApiSchema
                    {
                        Type = JsonSchemaType.Object,
                        AdditionalPropertiesAllowed = true
                    }
                }
            }
        };
    }

    private static Dictionary<string, IOpenApiExample> CreateRequestExamples()
    {
        return new Dictionary<string, IOpenApiExample>
        {
            ["Movies"] = new OpenApiExample
            {
                Summary = "Query paged movies",
                Value = JsonNode.Parse(
                    """
                    {
                      "query": "query Movies($filters: MovieFiltersInput) { movies(filters: $filters) { items { id movieName releaseDate worldwideGross productionBudget domesticGross genres createdAt updatedAt } page pageSize totalCount totalPages sortBy sortDirection usedDefaultSort } }",
                      "variables": {
                        "filters": {
                          "search": "avatar",
                          "page": 1,
                          "pageSize": 10
                        }
                      }
                    }
                    """)
            },
            ["MovieById"] = new OpenApiExample
            {
                Summary = "Query a movie by ID",
                Value = JsonNode.Parse(
                    """
                    {
                      "query": "query MovieById($id: UUID!) { movieById(id: $id) { id movieName releaseDate worldwideGross productionBudget domesticGross genres createdAt updatedAt } }",
                      "variables": {
                        "id": "00000000-0000-0000-0000-000000000000"
                      }
                    }
                    """)
            },
            ["UpdateMovie"] = new OpenApiExample
            {
                Summary = "Patch a movie",
                Value = JsonNode.Parse(
                    """
                    {
                      "query": "mutation UpdateMovie($id: UUID!, $patch: MoviePatchRequestInput!) { updateMovie(id: $id, patch: $patch) { id movieName releaseDate worldwideGross productionBudget domesticGross genres createdAt updatedAt } }",
                      "variables": {
                        "id": "00000000-0000-0000-0000-000000000000",
                        "patch": {
                          "worldwideGross": 1111,
                          "productionBudget": 1
                        }
                      }
                    }
                    """)
            }
        };
    }
}
