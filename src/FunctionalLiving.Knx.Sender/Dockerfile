FROM mcr.microsoft.com/dotnet/runtime-deps:5.0.1

LABEL maintainer "David Cumps <david@cumps.be>"

COPY / /app
WORKDIR /app

EXPOSE 9001/tcp
ENV ASPNETCORE_URLS http://*:9001

ENTRYPOINT ["./FunctionalLiving.Knx.Sender"]
