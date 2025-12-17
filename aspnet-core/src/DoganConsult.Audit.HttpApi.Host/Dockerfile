FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/DoganConsult.Audit.HttpApi.Host/DoganConsult.Audit.HttpApi.Host.csproj", "src/DoganConsult.Audit.HttpApi.Host/"]
COPY ["src/DoganConsult.Audit.HttpApi/DoganConsult.Audit.HttpApi.csproj", "src/DoganConsult.Audit.HttpApi/"]
COPY ["src/DoganConsult.Audit.Application/DoganConsult.Audit.Application.csproj", "src/DoganConsult.Audit.Application/"]
COPY ["src/DoganConsult.Audit.Application.Contracts/DoganConsult.Audit.Application.Contracts.csproj", "src/DoganConsult.Audit.Application.Contracts/"]
COPY ["src/DoganConsult.Audit.EntityFrameworkCore/DoganConsult.Audit.EntityFrameworkCore.csproj", "src/DoganConsult.Audit.EntityFrameworkCore/"]
COPY ["src/DoganConsult.Audit.Domain/DoganConsult.Audit.Domain.csproj", "src/DoganConsult.Audit.Domain/"]
COPY ["src/DoganConsult.Audit.Domain.Shared/DoganConsult.Audit.Domain.Shared.csproj", "src/DoganConsult.Audit.Domain.Shared/"]
RUN dotnet restore "src/DoganConsult.Audit.HttpApi.Host/DoganConsult.Audit.HttpApi.Host.csproj"
COPY . .
WORKDIR "/src/src/DoganConsult.Audit.HttpApi.Host"
RUN dotnet build "DoganConsult.Audit.HttpApi.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DoganConsult.Audit.HttpApi.Host.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DoganConsult.Audit.HttpApi.Host.dll"]
