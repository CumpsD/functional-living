FROM mcr.microsoft.com/dotnet/runtime-deps:5.0.7

LABEL maintainer "David Cumps <david@cumps.be>"

COPY / /app
WORKDIR /app

EXPOSE 9000/tcp
ENV ASPNETCORE_URLS http://*:9000
ENV ASPNETCORE_ENVIRONMENT Production

ENTRYPOINT ["./FunctionalLiving.Api"]
