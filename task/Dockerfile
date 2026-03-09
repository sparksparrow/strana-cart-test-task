FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

COPY ["task.csproj", "./"]
RUN dotnet restore "task.csproj"

COPY . .

RUN dotnet build "task.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "task.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
RUN apk add --no-cache tzdata
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "task.dll"]
