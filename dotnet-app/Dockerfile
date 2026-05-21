# Stage 1 – build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY WebApp.csproj ./
RUN dotnet restore WebApp.csproj
COPY . .
RUN dotnet publish WebApp.csproj -c Release -o /app/publish --no-restore

# Stage 2 – runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "WebApp.dll"]
