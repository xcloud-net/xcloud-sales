#用编译环境编译项目
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
COPY . /app
WORKDIR /app
#dotnet 2.x不再需要手动restore，但是也可以手动restore来自定义源
#RUN dotnet restore
RUN dotnet publish ./XCloud.Sales/XCloud.Sales.Mall.Api/XCloud.Sales.Mall.Api.csproj -c Release -o ./out

#编译结果copy到runtime容器运行
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet","XCloud.Sales.Mall.Api.dll"]

#https://docs.microsoft.com/zh-cn/dotnet/core/tools/dotnet-publish?tabs=netcore21