FROM node:16.18.1 as build-stage
COPY . /app
WORKDIR /app

# set api gateway address
ENV gatewayAddress=http://localhost:7888

RUN npm install --registry=https://registry.npm.taobao.org
RUN npm run build
RUN cp -r /app/dist /app/store-manage-dist
RUN cp -r /app/dist /app/store-dist

#production stage
FROM nginx:1.15 as production-stage

WORKDIR /usr/share/nginx/html
RUN mkdir store
RUN mkdir store-manage
COPY --from=build-stage /app/store-manage-dist ./store-manage/
COPY --from=build-stage /app/store-dist ./store/

WORKDIR /etc/nginx/
RUN rm ./nginx.conf
COPY --from=build-stage /app/docker/nginx.conf ./nginx.conf

ENTRYPOINT ["nginx", "-g", "daemon off;"]
