FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app
COPY . ./
RUN dotnet restore 
RUN dotnet publish --no-self-contained -c Docker -o out -f net6.0 -r linux-x64 /p:VersionSuffix=beta /p:VersionPrefix=0.5.1 /p:Version=0.5.1 /p:FileVersion=0.5.1-beta
FROM mcr.microsoft.com/dotnet/runtime:6.0.6-focal-amd64
WORKDIR /app
COPY --from=build-env /app/out .
COPY --from=build-env /app/schemas ./schemas/
COPY --from=build-env /app/styles ./styles/
EXPOSE 5000/tcp
EXPOSE 7878/tcp
ENTRYPOINT ["dotnet", "agent.dll"]
CMD ["debug"]'
