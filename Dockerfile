# Use the official ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Use the official .NET SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["Api-Gandarias/Api-Gandarias.csproj", "Api-Gandarias/"]
COPY ["CC.Application/CC.Application.csproj", "CC.Application/"]
COPY ["CC.Domain/CC.Domain.csproj", "CC.Domain/"]
COPY ["CC.Infraestructure/CC.Infrastructure.csproj", "CC.Infraestructure/"]

# Restore dependencies
RUN dotnet restore "Api-Gandarias/Api-Gandarias.csproj"

# Copy all source code
COPY . .

# Build the application
WORKDIR "/src/Api-Gandarias"
RUN dotnet build "Api-Gandarias.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "Api-Gandarias.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment variables for App Runner
ENV ASPNETCORE_ENVIRONMENT=Production
ENV PORT=8080
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

# Run the application
ENTRYPOINT ["dotnet", "Api-Gandarias.dll"]