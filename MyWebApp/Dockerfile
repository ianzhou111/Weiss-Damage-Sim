# STEP 1: Build the React frontend
FROM node:18 AS frontend

WORKDIR /app
COPY ClientApp/ ./ClientApp/
WORKDIR /app/ClientApp
RUN npm install
RUN npm run build

# STEP 2: Build the ASP.NET Core backend
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src
COPY . .

# Restore and publish for Linux runtime
RUN dotnet restore
RUN dotnet publish -c Release -r linux-x64 --self-contained false -o /app/out


# STEP 3: Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

WORKDIR /app

RUN apt-get update && apt-get install -y \
    libfontconfig1 \
    libfreetype6 \
    libpng16-16 \
    libharfbuzz0b \
    libicu-dev \
    libpthread-stubs0-dev \
    libc6 \
    && rm -rf /var/lib/apt/lists/*

# ✅ Copy backend and frontend build output
COPY --from=build /app/out ./
COPY --from=frontend /app/ClientApp/dist ./wwwroot/

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "MyWebApp.dll"]
