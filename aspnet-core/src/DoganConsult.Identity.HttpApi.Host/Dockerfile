FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/DoganConsult.Identity.HttpApi.Host/DoganConsult.Identity.HttpApi.Host.csproj", "src/DoganConsult.Identity.HttpApi.Host/"]
COPY ["src/DoganConsult.Identity.HttpApi/DoganConsult.Identity.HttpApi.csproj", "src/DoganConsult.Identity.HttpApi/"]
COPY ["src/DoganConsult.Identity.Application/DoganConsult.Identity.Application.csproj", "src/DoganConsult.Identity.Application/"]
COPY ["src/DoganConsult.Identity.Application.Contracts/DoganConsult.Identity.Application.Contracts.csproj", "src/DoganConsult.Identity.Application.Contracts/"]
COPY ["src/DoganConsult.Identity.EntityFrameworkCore/DoganConsult.Identity.EntityFrameworkCore.csproj", "src/DoganConsult.Identity.EntityFrameworkCore/"]
COPY ["src/DoganConsult.Identity.Domain/DoganConsult.Identity.Domain.csproj", "src/DoganConsult.Identity.Domain/"]
COPY ["src/DoganConsult.Identity.Domain.Shared/DoganConsult.Identity.Domain.Shared.csproj", "src/DoganConsult.Identity.Domain.Shared/"]
RUN dotnet restore "src/DoganConsult.Identity.HttpApi.Host/DoganConsult.Identity.HttpApi.Host.csproj"
COPY . .
WORKDIR "/src/src/DoganConsult.Identity.HttpApi.Host"
RUN dotnet build "DoganConsult.Identity.HttpApi.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DoganConsult.Identity.HttpApi.Host.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DoganConsult.Identity.HttpApi.Host.dll"]
