# ---------- Etapa 1: compilar con el SDK ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos primero los .csproj para aprovechar la caché de "restore"
COPY Lockton.sln .
COPY Lockton.Api/Lockton.Api.csproj Lockton.Api/
COPY Lockton.Services/Lockton.Services.csproj Lockton.Services/
COPY Lockton.Data/Lockton.Data.csproj Lockton.Data/
RUN dotnet restore Lockton.Api/Lockton.Api.csproj

# Copiamos el resto del código y publicamos
COPY . .
RUN dotnet publish Lockton.Api/Lockton.Api.csproj -c Release -o /app/publish --no-restore

# ---------- Etapa 2: imagen liviana solo con el runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
# Usuario sin privilegios incluido en la imagen oficial
USER app
ENTRYPOINT ["dotnet", "Lockton.Api.dll"]
