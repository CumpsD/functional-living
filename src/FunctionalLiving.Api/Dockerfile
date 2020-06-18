FROM mcr.microsoft.com/dotnet/core/runtime-deps:3.1.5

LABEL maintainer "David Cumps <david@cumps.be>"

COPY / /app
WORKDIR /app

EXPOSE 9000/tcp
ENV ASPNETCORE_URLS http://*:9000

ENTRYPOINT ["./FunctionalLiving.Api"]
