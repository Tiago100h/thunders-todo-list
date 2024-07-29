FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ThundersTodoList.Api/ThundersTodoList.Api.csproj", "ThundersTodoList.Api/"]
COPY ["ThundersTodoList.Domain/ThundersTodoList.Domain.csproj", "ThundersTodoList.Domain/"]
COPY ["ThundersTodoList.Infra/ThundersTodoList.Infra.csproj", "ThundersTodoList.Infra/"]
COPY ["ThundersTodoList.Service/ThundersTodoList.Service.csproj", "ThundersTodoList.Service/"]
RUN dotnet restore "ThundersTodoList.Api/ThundersTodoList.Api.csproj"
COPY . .
WORKDIR "/src/ThundersTodoList.Api"
RUN dotnet build "ThundersTodoList.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ThundersTodoList.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ThundersTodoList.Api.dll"]