FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/DoganConsult.Workspace.HttpApi.Host/DoganConsult.Workspace.HttpApi.Host.csproj", "src/DoganConsult.Workspace.HttpApi.Host/"]
COPY ["src/DoganConsult.Workspace.HttpApi/DoganConsult.Workspace.HttpApi.csproj", "src/DoganConsult.Workspace.HttpApi/"]
COPY ["src/DoganConsult.Workspace.Application/DoganConsult.Workspace.Application.csproj", "src/DoganConsult.Workspace.Application/"]
COPY ["src/DoganConsult.Workspace.Application.Contracts/DoganConsult.Workspace.Application.Contracts.csproj", "src/DoganConsult.Workspace.Application.Contracts/"]
COPY ["src/DoganConsult.Workspace.EntityFrameworkCore/DoganConsult.Workspace.EntityFrameworkCore.csproj", "src/DoganConsult.Workspace.EntityFrameworkCore/"]
COPY ["src/DoganConsult.Workspace.Domain/DoganConsult.Workspace.Domain.csproj", "src/DoganConsult.Workspace.Domain/"]
COPY ["src/DoganConsult.Workspace.Domain.Shared/DoganConsult.Workspace.Domain.Shared.csproj", "src/DoganConsult.Workspace.Domain.Shared/"]
RUN dotnet restore "src/DoganConsult.Workspace.HttpApi.Host/DoganConsult.Workspace.HttpApi.Host.csproj"
COPY . .
WORKDIR "/src/src/DoganConsult.Workspace.HttpApi.Host"
RUN dotnet build "DoganConsult.Workspace.HttpApi.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DoganConsult.Workspace.HttpApi.Host.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DoganConsult.Workspace.HttpApi.Host.dll"]
