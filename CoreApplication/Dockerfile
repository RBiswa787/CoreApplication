FROM mcr.microsoft.com/dotnet/aspnet:5.0-focal AS base
WORKDIR /app
EXPOSE 80
FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS build
WORKDIR /src
COPY [".csproj", "./"]
RUN dotnet restore "./CoreApplication.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "CoreApplication.csproj" -c Release -o /app/build
FROM build AS publish
RUN dotnet publish "CoreApplication.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CoreApplication.dll"]