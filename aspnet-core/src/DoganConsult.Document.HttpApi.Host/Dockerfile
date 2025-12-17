FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/DoganConsult.Document.HttpApi.Host/DoganConsult.Document.HttpApi.Host.csproj", "src/DoganConsult.Document.HttpApi.Host/"]
COPY ["src/DoganConsult.Document.HttpApi/DoganConsult.Document.HttpApi.csproj", "src/DoganConsult.Document.HttpApi/"]
COPY ["src/DoganConsult.Document.Application/DoganConsult.Document.Application.csproj", "src/DoganConsult.Document.Application/"]
COPY ["src/DoganConsult.Document.Application.Contracts/DoganConsult.Document.Application.Contracts.csproj", "src/DoganConsult.Document.Application.Contracts/"]
COPY ["src/DoganConsult.Document.EntityFrameworkCore/DoganConsult.Document.EntityFrameworkCore.csproj", "src/DoganConsult.Document.EntityFrameworkCore/"]
COPY ["src/DoganConsult.Document.Domain/DoganConsult.Document.Domain.csproj", "src/DoganConsult.Document.Domain/"]
COPY ["src/DoganConsult.Document.Domain.Shared/DoganConsult.Document.Domain.Shared.csproj", "src/DoganConsult.Document.Domain.Shared/"]
RUN dotnet restore "src/DoganConsult.Document.HttpApi.Host/DoganConsult.Document.HttpApi.Host.csproj"
COPY . .
WORKDIR "/src/src/DoganConsult.Document.HttpApi.Host"
RUN dotnet build "DoganConsult.Document.HttpApi.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DoganConsult.Document.HttpApi.Host.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DoganConsult.Document.HttpApi.Host.dll"]
