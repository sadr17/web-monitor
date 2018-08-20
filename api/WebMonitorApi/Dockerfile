FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY *.sln ./
COPY ../fix_test/api/WebMonitorApi/WebMonitorApi.csproj ../fix_test/api/WebMonitorApi/
RUN dotnet restore
COPY . .
WORKDIR /src/../fix_test/api/WebMonitorApi
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebMonitorApi.dll"]
