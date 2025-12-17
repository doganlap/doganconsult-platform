FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/DoganConsult.AI.HttpApi.Host/DoganConsult.AI.HttpApi.Host.csproj", "src/DoganConsult.AI.HttpApi.Host/"]
COPY ["src/DoganConsult.AI.HttpApi/DoganConsult.AI.HttpApi.csproj", "src/DoganConsult.AI.HttpApi/"]
COPY ["src/DoganConsult.AI.Application/DoganConsult.AI.Application.csproj", "src/DoganConsult.AI.Application/"]
COPY ["src/DoganConsult.AI.Application.Contracts/DoganConsult.AI.Application.Contracts.csproj", "src/DoganConsult.AI.Application.Contracts/"]
COPY ["src/DoganConsult.AI.EntityFrameworkCore/DoganConsult.AI.EntityFrameworkCore.csproj", "src/DoganConsult.AI.EntityFrameworkCore/"]
COPY ["src/DoganConsult.AI.Domain/DoganConsult.AI.Domain.csproj", "src/DoganConsult.AI.Domain/"]
COPY ["src/DoganConsult.AI.Domain.Shared/DoganConsult.AI.Domain.Shared.csproj", "src/DoganConsult.AI.Domain.Shared/"]
COPY ["src/DoganConsult.AI.Infrastructure/DoganConsult.AI.Infrastructure.csproj", "src/DoganConsult.AI.Infrastructure/"]
RUN dotnet restore "src/DoganConsult.AI.HttpApi.Host/DoganConsult.AI.HttpApi.Host.csproj"
COPY . .
WORKDIR "/src/src/DoganConsult.AI.HttpApi.Host"
RUN dotnet build "DoganConsult.AI.HttpApi.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DoganConsult.AI.HttpApi.Host.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DoganConsult.AI.HttpApi.Host.dll"]
