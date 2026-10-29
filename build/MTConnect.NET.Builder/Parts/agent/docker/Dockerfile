# MTConnect.NET Agent Docker Image
#
# Built by the Builder ('agent docker build') from a context it prepares in [projectPath]/bin/docker/[tag]:
#   agent/                  The Agent published for the image's Runtime (framework-dependent)
#   docker-healthcheck.sh
#
# The Base Image must match the Runtime it was published for (ex. an Alpine image needs a linux-musl-* Runtime)

ARG BASE_IMAGE=mcr.microsoft.com/dotnet/runtime:8.0
FROM ${BASE_IMAGE}

ARG VERSION
LABEL org.opencontainers.image.title="MTConnect.NET Agent" \
      org.opencontainers.image.version="${VERSION}" \
      org.opencontainers.image.vendor="TrakHound Inc." \
      org.opencontainers.image.licenses="MIT" \
      org.opencontainers.image.source="https://github.com/TrakHound/MTConnect.NET"

USER root

# Alpine Base Images don't include the ICU libraries used for globalization
RUN if command -v apk > /dev/null 2>&1; then apk add --no-cache icu-libs; fi
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# The Agent writes logs and buffers to its program directory, so it's owned by the Agent's user ('app', created by the Base Image)
COPY --chown=app:app agent/ /app/
COPY docker-healthcheck.sh /usr/local/bin/
RUN chmod 755 /usr/local/bin/docker-healthcheck.sh

USER app
WORKDIR /app

# HTTP Server, MQTT Broker and SHDR Adapter Ports
EXPOSE 5000/tcp
EXPOSE 1883/tcp
EXPOSE 7878/tcp

# Requests the HTTP Server's Probe (set MTCONNECT_HEALTHCHECK_PORT if the http-server module uses another Port)
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 CMD ["/usr/local/bin/docker-healthcheck.sh"]

ENTRYPOINT ["dotnet", "agent.dll"]
CMD ["debug"]
