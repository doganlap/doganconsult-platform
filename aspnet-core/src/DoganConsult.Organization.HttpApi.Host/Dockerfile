FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/DoganConsult.Organization.HttpApi.Host/DoganConsult.Organization.HttpApi.Host.csproj", "src/DoganConsult.Organization.HttpApi.Host/"]
COPY ["src/DoganConsult.Organization.HttpApi/DoganConsult.Organization.HttpApi.csproj", "src/DoganConsult.Organization.HttpApi/"]
COPY ["src/DoganConsult.Organization.Application/DoganConsult.Organization.Application.csproj", "src/DoganConsult.Organization.Application/"]
COPY ["src/DoganConsult.Organization.Application.Contracts/DoganConsult.Organization.Application.Contracts.csproj", "src/DoganConsult.Organization.Application.Contracts/"]
COPY ["src/DoganConsult.Organization.EntityFrameworkCore/DoganConsult.Organization.EntityFrameworkCore.csproj", "src/DoganConsult.Organization.EntityFrameworkCore/"]
COPY ["src/DoganConsult.Organization.Domain/DoganConsult.Organization.Domain.csproj", "src/DoganConsult.Organization.Domain/"]
COPY ["src/DoganConsult.Organization.Domain.Shared/DoganConsult.Organization.Domain.Shared.csproj", "src/DoganConsult.Organization.Domain.Shared/"]
RUN dotnet restore "src/DoganConsult.Organization.HttpApi.Host/DoganConsult.Organization.HttpApi.Host.csproj"
COPY . .
WORKDIR "/src/src/DoganConsult.Organization.HttpApi.Host"
RUN dotnet build "DoganConsult.Organization.HttpApi.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DoganConsult.Organization.HttpApi.Host.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DoganConsult.Organization.HttpApi.Host.dll"]
