FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
ENV ASPNETCORE_URLS http://*:7500
EXPOSE 7500
COPY . ./
ENTRYPOINT ["dotnet", "GeneratorPP.dll"]
