FROM maven:3.6-openjdk-11 AS build-env
COPY . /app

# build environment
WORKDIR /app/api-gateway
RUN mvn clean package

# runtime
FROM openjdk:11-jre
WORKDIR /app
COPY --from=build-env /app/api-gateway/target/ .
ENTRYPOINT ["java","-jar","api-gateway.jar"]