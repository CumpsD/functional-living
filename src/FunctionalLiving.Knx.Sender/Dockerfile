FROM mcr.microsoft.com/dotnet/runtime-deps:7.0.3

LABEL maintainer "David Cumps <david@cumps.be>"

COPY / /app
WORKDIR /app

EXPOSE 9001/tcp
ENV ASPNETCORE_URLS http://*:9001
ENV ASPNETCORE_ENVIRONMENT Production

ENTRYPOINT ["./FunctionalLiving.Knx.Sender"]
