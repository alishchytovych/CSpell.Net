FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base

WORKDIR /app
EXPOSE 5002

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["SpellChecker/src/SpellChecker.csproj", "SpellChecker/src/"]
RUN dotnet restore "SpellChecker/src/SpellChecker.csproj"
COPY . .
WORKDIR "/src/SpellChecker/src"
RUN dotnet build "SpellChecker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SpellChecker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SpellChecker.dll"]
