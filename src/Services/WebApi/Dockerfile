FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Services/WebApi/WebApi.csproj", "src/Services/WebApi/"]
COPY ["src/Modules/GrainInterfaces/GrainInterfaces.csproj", "src/Modules/GrainInterfaces/"]
COPY ["src/Modules/Core/Core.csproj", "src/Modules/Core/"]
COPY ["src/Modules/Discord.Shared/Discord.Shared.csproj", "src/Modules/Discord.Shared/"]
COPY ["src/Modules/Wow.Shared/Wow.Shared.csproj", "src/Modules/Wow.Shared/"]
COPY ["src/Modules/Shared/Shared.csproj", "src/Modules/Shared/"]
COPY ["src/Modules/Infrastructure/Infrastructure.csproj", "src/Modules/Infrastructure/"]
COPY ["src/Modules/Discord/Discord.csproj", "src/Modules/Discord/"]
COPY ["src/Modules/Grains/Grains.csproj", "src/Modules/Grains/"]
COPY ["src/Modules/Wow/Wow.csproj", "src/Modules/Wow/"]
COPY ["src/Modules/Streams/Streams.csproj", "src/Modules/Streams/"]
RUN dotnet restore "src/Services/WebApi/WebApi.csproj"
COPY . .
WORKDIR "/src/src/Services/WebApi"
RUN dotnet build "WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebApi.csproj" -c Release -o /app/publish

FROM base AS final
LABEL org.opencontainers.image.source = "https://github.com/Avabin/PierogiesBot"
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApi.dll"]
