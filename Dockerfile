FROM mcr.microsoft.com/dotnet/sdk:8.0@sha256:35792ea4ad1db051981f62b313f1be3b46b1f45cadbaa3c288cd0d3056eefb83 AS build-env
WORKDIR /App

COPY WebShopSolution.sln ./
COPY WebShop.Common/WebShop.Common.csproj WebShop.Common/
COPY WebShop.SQL/WebShop.SQL.csproj WebShop.SQL/
COPY WebShopTests/WebShopTests.csproj WebShopTests/
COPY WebShop/WebShop.csproj WebShop/

RUN dotnet restore WebShopSolution.sln

COPY . ./
RUN dotnet test WebShopTests/WebShopTests.csproj --no-build --verbosity normal
RUN dotnet publish WebShop/WebShop.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0@sha256:6c4df091e4e531bb93bdbfe7e7f0998e7ced344f54426b7e874116a3dc3233ff
WORKDIR /App
COPY --from=build-env /App/out ./

EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "WebShop.dll"]