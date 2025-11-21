FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["API_Investimentos.Api/API_Investimentos.Api.csproj", "API_Investimentos.Api/"]
COPY ["API_Investimentos.Application/API_Investimentos.Application.csproj", "API_Investimentos.Application/"]
COPY ["API_Investimentos.Domain/API_Investimentos.Domain.csproj", "API_Investimentos.Domain/"]
COPY ["API_Investimentos.Infrastructure/API_Investimentos.Infrastructure.csproj", "API_Investimentos.Infrastructure/"]
COPY ["Shared/Shared.csproj", "Shared/"]
RUN dotnet restore "API_Investimentos.Api/API_Investimentos.Api.csproj"
COPY . .
WORKDIR "/src/API_Investimentos.Api"
RUN dotnet build "API_Investimentos.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "API_Investimentos.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "API_Investimentos.Api.dll"]
