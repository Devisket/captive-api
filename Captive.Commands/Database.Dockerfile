FROM mysql:latest
WORKDIR /usr/src/app

COPY ./scripts/database /docker-entrypoint-initdb.d
EXPOSE 3306
