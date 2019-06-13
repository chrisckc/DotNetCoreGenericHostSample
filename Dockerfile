FROM microsoft/dotnet:2.2-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY DotNetCoreGenericHostSample.csproj ./
RUN dotnet restore ./DotNetCoreGenericHostSample.csproj
COPY . .
WORKDIR /src/.
RUN dotnet build DotNetCoreGenericHostSample.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish DotNetCoreGenericHostSample.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "DotNetCoreGenericHostSample.dll"]
