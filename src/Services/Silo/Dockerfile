FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Services/Silo/Silo.csproj", "Silo/"]
COPY ["src/Modules/Grains/Grains.csproj", "Grains/"]
COPY ["src/Modules/Core/Core.csproj", "Core/"]
COPY ["src/Modules/GrainInterfaces/GrainInterfaces.csproj", "GrainInterfaces/"]
COPY ["src/Modules/Wow.Shared/Wow.Shared.csproj", "Wow.Shared/"]
COPY ["src/Modules/Wow/Wow.csproj", "Wow/"]
COPY ["src/Modules/Discord/Discord.csproj", "Discord/"]
COPY ["src/Modules/Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["src/Modules/Streams/Streams.csproj", "Streams/"]
RUN dotnet restore "src/Services/Silo/Silo.csproj"
COPY . .
WORKDIR "/src/Silo"
RUN dotnet build "Silo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Silo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV DOTNET_SiloType=All
ENTRYPOINT ["dotnet", "Silo.dll"]
