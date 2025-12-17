FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/DoganConsult.UserProfile.HttpApi.Host/DoganConsult.UserProfile.HttpApi.Host.csproj", "src/DoganConsult.UserProfile.HttpApi.Host/"]
COPY ["src/DoganConsult.UserProfile.HttpApi/DoganConsult.UserProfile.HttpApi.csproj", "src/DoganConsult.UserProfile.HttpApi/"]
COPY ["src/DoganConsult.UserProfile.Application/DoganConsult.UserProfile.Application.csproj", "src/DoganConsult.UserProfile.Application/"]
COPY ["src/DoganConsult.UserProfile.Application.Contracts/DoganConsult.UserProfile.Application.Contracts.csproj", "src/DoganConsult.UserProfile.Application.Contracts/"]
COPY ["src/DoganConsult.UserProfile.EntityFrameworkCore/DoganConsult.UserProfile.EntityFrameworkCore.csproj", "src/DoganConsult.UserProfile.EntityFrameworkCore/"]
COPY ["src/DoganConsult.UserProfile.Domain/DoganConsult.UserProfile.Domain.csproj", "src/DoganConsult.UserProfile.Domain/"]
COPY ["src/DoganConsult.UserProfile.Domain.Shared/DoganConsult.UserProfile.Domain.Shared.csproj", "src/DoganConsult.UserProfile.Domain.Shared/"]
RUN dotnet restore "src/DoganConsult.UserProfile.HttpApi.Host/DoganConsult.UserProfile.HttpApi.Host.csproj"
COPY . .
WORKDIR "/src/src/DoganConsult.UserProfile.HttpApi.Host"
RUN dotnet build "DoganConsult.UserProfile.HttpApi.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DoganConsult.UserProfile.HttpApi.Host.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DoganConsult.UserProfile.HttpApi.Host.dll"]
