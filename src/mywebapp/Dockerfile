FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS builder
WORKDIR /src

COPY ["mywebapp.csproj", "./"]
RUN dotnet restore "mywebapp.csproj"

COPY . .

RUN dotnet publish "mywebapp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final
WORKDIR /app

EXPOSE 8000

ENV ASPNETCORE_URLS=http://+:8000

COPY --from=builder /app/publish .

ENTRYPOINT ["dotnet", "mywebapp.dll"]