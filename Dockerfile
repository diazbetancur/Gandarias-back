# ASP.NET Core 8.0 - App Runner ready
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Api-Gandarias/Api-Gandarias.csproj", "Api-Gandarias/"]
COPY ["CC.Application/CC.Application.csproj", "CC.Application/"]
COPY ["CC.Domain/CC.Domain.csproj", "CC.Domain/"]
COPY ["CC.Infraestructure/CC.Infrastructure.csproj", "CC.Infraestructure/"]
RUN dotnet restore "Api-Gandarias/Api-Gandarias.csproj"

COPY . .
WORKDIR "/src/Api-Gandarias"
RUN dotnet build "Api-Gandarias.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Api-Gandarias.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENV PORT=8080

ENTRYPOINT ["dotnet", "Api-Gandarias.dll"]