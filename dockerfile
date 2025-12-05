# Stage 1: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy csproj and restore as distinct layers
COPY *.sln .
COPY WeTrust.Api/*.csproj ./WeTrust.Api/
RUN dotnet restore

# copy everything else and build
COPY WeTrust.Api/. ./WeTrust.Api/
WORKDIR /src/WeTrust.Api
RUN dotnet publish -c Release -o /app/publish

# Stage 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
# Expose port used by Render
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "WeTrust.Api.dll"]
