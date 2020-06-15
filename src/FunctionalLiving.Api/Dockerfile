FROM mcr.microsoft.com/dotnet/core/runtime-deps:3.1.4

LABEL maintainer "David Cumps <david@cumps.be>"

COPY / /app
WORKDIR /app

EXPOSE 5000/tcp
ENV ASPNETCORE_URLS http://*:5000

ENTRYPOINT ["./FunctionalLiving.Api"]
