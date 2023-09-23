FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-api

RUN apt-get update && \
    apt-get install -y openssl iproute2 ca-certificates && \
    apt-get clean

WORKDIR /src

COPY ./ ./

RUN dotnet restore "/src/Server/Server.csproj"
RUN dotnet publish "/src/Server/Server.csproj" -c Release -o /app --no-restore --disable-parallel

#FROM node:19.6.1 AS build-web

#WORKDIR /web

#COPY ./web /web

#RUN npm install
#RUN npm run static

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime

WORKDIR /app

COPY --from=build-api /app .
#COPY --from=build-web /web/out ./web

ENV ASPNETCORE_ENVIRONMENT Production

ENV TZ=Europe/Berlin
ENV LANG de_DE.UTF-8
ENV LANGUAGE ${LANG}
ENV LC_ALL ${LANG}

EXPOSE 80/tpc
EXPOSE 8080/udp

ENTRYPOINT [ "dotnet", "BetterBeatSaber.Server.dll" ]