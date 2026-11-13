# MTConnect.NET Agent Docker Image, built from source
#
#   docker build -t mtconnect.net-agent .
#   docker run -p 5000:5000 -p 7878:7878 mtconnect.net-agent
#
# Released images (trakhound/mtconnect.net-agent on Docker Hub) are built by TrakHound's release builder;
# this file builds an equivalent image from a checkout of this repository.
#
# FRAMEWORK selects the Agent's target framework, and RUNTIME_IMAGE the base image it runs on (they must match)

ARG FRAMEWORK=net8.0
ARG RUNTIME_IMAGE=mcr.microsoft.com/dotnet/runtime:8.0

# The libraries multi-target up to net10.0, so restoring them needs the .NET 10 SDK even when the Agent targets net8.0
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG FRAMEWORK
WORKDIR /src
COPY . .
RUN dotnet publish agent/MTConnect.NET-Agent/MTConnect.NET-Agent.csproj \
    -c Release -f ${FRAMEWORK} --no-self-contained -o /app

FROM ${RUNTIME_IMAGE}
LABEL org.opencontainers.image.title="MTConnect.NET Agent" \
      org.opencontainers.image.licenses="MIT" \
      org.opencontainers.image.source="https://github.com/TrakHound/MTConnect.NET"

# The Agent writes logs and buffers to its program directory, so it's owned by the Agent's user ('app', created by the Base Image)
COPY --from=build --chown=app:app /app /app

USER app
WORKDIR /app

# HTTP Server, MQTT Broker and SHDR Adapter Ports
EXPOSE 5000/tcp
EXPOSE 1883/tcp
EXPOSE 7878/tcp

# Checks that the HTTP Server accepts connections (the runtime image has bash, but not curl or wget)
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
    CMD ["bash", "-c", "exec 3<>/dev/tcp/127.0.0.1/5000"]

ENTRYPOINT ["dotnet", "agent.dll"]
CMD ["debug"]
