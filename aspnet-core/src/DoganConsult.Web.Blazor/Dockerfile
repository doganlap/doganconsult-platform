FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/DoganConsult.Web.Blazor/DoganConsult.Web.Blazor.csproj", "src/DoganConsult.Web.Blazor/"]
COPY ["src/DoganConsult.Web.Application/DoganConsult.Web.Application.csproj", "src/DoganConsult.Web.Application/"]
COPY ["src/DoganConsult.Web.Application.Contracts/DoganConsult.Web.Application.Contracts.csproj", "src/DoganConsult.Web.Application.Contracts/"]
COPY ["src/DoganConsult.Web.EntityFrameworkCore/DoganConsult.Web.EntityFrameworkCore.csproj", "src/DoganConsult.Web.EntityFrameworkCore/"]
COPY ["src/DoganConsult.Web.Domain/DoganConsult.Web.Domain.csproj", "src/DoganConsult.Web.Domain/"]
COPY ["src/DoganConsult.Web.Domain.Shared/DoganConsult.Web.Domain.Shared.csproj", "src/DoganConsult.Web.Domain.Shared/"]
COPY ["src/DoganConsult.Web.HttpApi/DoganConsult.Web.HttpApi.csproj", "src/DoganConsult.Web.HttpApi/"]
RUN dotnet restore "src/DoganConsult.Web.Blazor/DoganConsult.Web.Blazor.csproj"
COPY . .
WORKDIR "/src/src/DoganConsult.Web.Blazor"
RUN dotnet build "DoganConsult.Web.Blazor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DoganConsult.Web.Blazor.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DoganConsult.Web.Blazor.dll"]
