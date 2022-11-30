FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
COPY ["src/Services/Silo/Silo.csproj", "src/Services/Silo/"]
COPY ["src/Modules/Discord.Commands/Discord.Commands.csproj", "src/Modules/Discord.Commands/"]
COPY ["src/Modules/GrainInterfaces/GrainInterfaces.csproj", "src/Modules/GrainInterfaces/"]
COPY ["src/Modules/Discord.Shared/Discord.Shared.csproj", "src/Modules/Discord.Shared/"]
COPY ["src/Modules/Wow.Shared/Wow.Shared.csproj", "src/Modules/Wow.Shared/"]
COPY ["src/Modules/Core/Core.csproj", "src/Modules/Core/"]
COPY ["src/Modules/Discord/Discord.csproj", "src/Modules/Discord/"]
COPY ["src/Modules/Infrastructure/Infrastructure.csproj", "src/Modules/Infrastructure/"]
COPY ["src/Modules/Grains/Grains.csproj", "src/Modules/Grains/"]
COPY ["src/Modules/Wow/Wow.csproj", "src/Modules/Wow/"]
COPY ["src/Modules/Streams/Streams.csproj", "src/Modules/Streams/"]
RUN dotnet restore "src/Services/Silo/Silo.csproj"
COPY . .
WORKDIR "/src/Services/Silo"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Silo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Silo.dll"]
