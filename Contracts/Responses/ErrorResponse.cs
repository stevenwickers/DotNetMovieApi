namespace DotNetMovieApi.Contracts.Responses;

public sealed class ErrorResponse
{
    public required string Message { get; init; }
    public string? CorrelationId { get; init; }
    public object? Detail { get; init; }
}