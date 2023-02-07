#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
ARG VERSION=1.0.0
ARG INFORMAL_VERSION=1.0.0-UNDEFINED

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["./nuget.config", "."]
COPY ["./WebApp/WebApp.csproj", "WebApp/"]
COPY ["./ShopBase/ShopBase.csproj", "ShopBase/"]
RUN dotnet restore "WebApp/WebApp.csproj"
COPY . .
WORKDIR "/src/WebApp"
RUN dotnet build "WebApp.csproj" -c Release -o /app/build

FROM build AS publish
ARG VERSION
ARG INFORMAL_VERSION
ENV ENV_VERSION=$VERSION
ENV ENV_INFORMAL_VERSION=$INFORMAL_VERSION
RUN dotnet publish "WebApp.csproj" -c Release -o /app/publish /p:InformationalVersion="${ENV_INFORMAL_VERSION}" /p:Version="${ENV_VERSION}" /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApp.dll"]