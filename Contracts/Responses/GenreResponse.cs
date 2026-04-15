namespace DotNetMovieApi.Contracts.Responses;

public class GenreResponse
{
    public Guid Id { get; set;  }
    public required string Name { get; set; }
}
