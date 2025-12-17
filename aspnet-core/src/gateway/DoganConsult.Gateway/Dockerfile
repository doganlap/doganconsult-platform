FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/gateway/DoganConsult.Gateway/DoganConsult.Gateway.csproj", "src/gateway/DoganConsult.Gateway/"]
RUN dotnet restore "src/gateway/DoganConsult.Gateway/DoganConsult.Gateway.csproj"
COPY . .
WORKDIR "/src/src/gateway/DoganConsult.Gateway"
RUN dotnet build "DoganConsult.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DoganConsult.Gateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DoganConsult.Gateway.dll"]
