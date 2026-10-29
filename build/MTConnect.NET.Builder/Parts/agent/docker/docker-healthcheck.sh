#!/bin/sh
# MTConnect.NET Agent Docker Health Check
#
# Requests the Agent's Probe from the http-server module (Port 5000 unless MTCONNECT_HEALTHCHECK_PORT is set). The .NET Base
# Images don't include curl, so this uses wget (Alpine), curl, or bash's /dev/tcp (Debian and Ubuntu), whichever is available

port="${MTCONNECT_HEALTHCHECK_PORT:-5000}"
url="http://127.0.0.1:$port/probe"

if command -v wget > /dev/null 2>&1; then
    exec wget -q -T 5 -O /dev/null "$url"
elif command -v curl > /dev/null 2>&1; then
    exec curl -fsS -m 5 -o /dev/null "$url"
elif command -v bash > /dev/null 2>&1; then
    exec bash -c 'exec 3<>"/dev/tcp/127.0.0.1/$0" || exit 1
printf "GET /probe HTTP/1.0\r\nHost: localhost\r\n\r\n" >&3
read -r version status rest <&3
[ "$status" = "200" ]' "$port"
fi

echo "No HTTP client (wget, curl or bash) available for the health check" >&2
exit 1
