FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["dotnetChess/dotnetChess.csproj", "dotnetChess/"]
RUN dotnet restore "dotnetChess/dotnetChess.csproj"
COPY . .
WORKDIR "/src/dotnetChess"
RUN dotnet build "dotnetChess.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "dotnetChess.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "dotnetChess.dll"]
