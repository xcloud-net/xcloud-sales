# 生产环境部署方案参考

> 方案只供参考

因为正式部署包含密钥等保密信息，因此在git提交前已经抹去。
所以此文件夹中的compose直接部署会存在配置相关的错误。

## 0. 发布前准备

```shell
# 安装docker，docker-compose
# 创建网络
$ docker network create xx
```

## 1. 部署基础环境

```shell
$ cd ./services
$ docker-compose up -d
```
### 1.1 部署apm service【如果不要可以跳过】

```shell
$ cd ./skywalking
$ docker-compose up -d
```

## 2. 部署api

```shell
$ cd ./api
$ docker-compose up -d
```

## 3. 部署前端ui

```shell
$ cd ./web-ui
$ docker-compose up -d
```

## 4. 部署nginx反向代理，配置证书

```shell
$ cd ./nginx
$ docker-compose up -d
```