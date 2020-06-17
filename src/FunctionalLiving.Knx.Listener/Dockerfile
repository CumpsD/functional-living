FROM mcr.microsoft.com/dotnet/core/runtime-deps:3.1.4

LABEL maintainer "David Cumps <david@cumps.be>"

COPY / /app
WORKDIR /app

ENTRYPOINT ["./FunctionalLiving.Knx.Listener"]
