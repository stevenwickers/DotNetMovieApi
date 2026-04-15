dotnet clean
dotnet nuget locals all --clear
dotnet restore
dotnet build
dotnet run