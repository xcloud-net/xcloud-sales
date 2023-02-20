# 测试环境项目预览

> 项目第一次启动会自动编译代码到容器，时间会比较长
> 如果代码修改也要重新编译

> 在编译时请保证充足的计算机性能，umijs项目需要足够内存才能打包成功。

### 一行命令
```shell
# create if not exist
$ docker network create xx
# start containers
$ docker-compose up -d
# build anyway
$ docker-compose up -d --build
```

### 单独编译
```shell
# 由于完全编译时间比较长，不排除因为网络或者其他因素出错的可能。
# 或是因为单独修改了某个项目，需要重新打包。
# 因此可以选择单独编译，比如：
$ docker-compose build platform-api
$ docker-compose build mall-api
$ docker-compose build api-gateway
$ docker-compose build web-ui
# 最后，启动服务
$ docker-compose up -d
```


### 访问前端
http://localhost:9999/store/


### 访问后台
http://localhost:9999/store/manage/

> 因为测试环境没有配置nginx，因此直接访问根目录不会自动跳转