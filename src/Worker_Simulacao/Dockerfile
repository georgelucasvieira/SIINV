FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Worker_Simulacao/Worker_Simulacao.csproj", "Worker_Simulacao/"]
RUN dotnet restore "Worker_Simulacao/Worker_Simulacao.csproj"
COPY . .
WORKDIR "/src/Worker_Simulacao"
RUN dotnet build "Worker_Simulacao.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Worker_Simulacao.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Worker_Simulacao.dll"]
