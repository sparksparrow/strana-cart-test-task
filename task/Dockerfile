FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src
COPY ["task.csproj", "task/"]
RUN dotnet restore "task/task.csproj"
COPY . .
WORKDIR "/src/task"
RUN dotnet build "task.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "task.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "task.dll"]
