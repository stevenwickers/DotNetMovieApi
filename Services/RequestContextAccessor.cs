namespace DotNetMovieApi.Services;

public sealed class RequestContextAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RequestContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? CorrelationId =>
        _httpContextAccessor.HttpContext?.Items[Middleware.CorrelationIdMiddleware.ItemKey]?.ToString();
}