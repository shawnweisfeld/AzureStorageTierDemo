#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS base
WORKDIR /app

# copy csproj and restore as distinct layers
COPY . ./
RUN dotnet restore

# copy and build everything else
COPY . ./
RUN dotnet publish ./AzureStorageTierDemo/AzureStorageTierDemo.csproj -c Release -o out
ENTRYPOINT ["dotnet", "out/AzureStorageTierDemo.dll"]