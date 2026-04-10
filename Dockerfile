FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

COPY ["TramAnh_WMS.csproj", "./"]
RUN dotnet restore "TramAnh_WMS.csproj"

COPY . .
RUN dotnet publish "TramAnh_WMS.csproj" -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
ENV ASPNETCORE_URLS=http://+:${PORT}

ENTRYPOINT ["dotnet", "TramAnh_WMS.dll"]