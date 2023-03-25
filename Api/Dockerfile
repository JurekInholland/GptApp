FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
#EXPOSE 80
#EXPOSE 443

FROM node:19-alpine AS build-ui
WORKDIR /app
COPY ./Ui .
RUN npm install
RUN npm run build

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Api/Api.csproj", "Api/"]
RUN dotnet restore "Api/Api.csproj"
COPY . .
WORKDIR "/src/Api"
RUN dotnet build "Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build-ui /app/dist ./wwwroot
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 \
  DOTNET_RUNNING_IN_CONTAINER=true \
  ASPNETCORE_URLS=http://+:8080 \
ENTRYPOINT ["dotnet", "Api.dll", "--urls", "http://0.0.0.0:8080"]
