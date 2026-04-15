using Dapper;
using DotNetMovieApi.Data;
using DotNetMovieApi.Configuration;

var builder = WebApplication.CreateBuilder(args);

SqlMapper.AddTypeHandler(new DateOnlyHandler());
DefaultTypeMap.MatchNamesWithUnderscores = true;

builder.Services.AddSwaggerServices(builder.Configuration);
builder.Services.AddAppServices(builder.Configuration);

var app = builder.Build();
app.UseAppMiddleware();
app.MapAppEndpoints();

app.Run();
