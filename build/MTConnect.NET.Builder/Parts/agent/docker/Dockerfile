FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app
COPY . ./
FROM mcr.microsoft.com/dotnet/runtime:{DOCKER_BASE_IMAGE} AS base
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
USER app
WORKDIR /app
COPY --from=build-env /app .
EXPOSE 5000/tcp
EXPOSE 1883/tcp
EXPOSE 7878/tcp
ENTRYPOINT ["dotnet", "agent.dll"]
CMD ["debug"]