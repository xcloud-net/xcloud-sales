#production stage
FROM nginx:1.15 as production-stage

COPY . /app

WORKDIR /etc/nginx/

RUN rm nginx.conf
RUN mv /app/nginx.conf ./nginx.conf

WORKDIR /usr/share/nginx/html
RUN mv /app/app/ .

ENTRYPOINT ["nginx", "-g", "daemon off;"]