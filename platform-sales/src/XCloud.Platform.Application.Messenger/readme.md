# 分布式socket服务器backend

- 广播
- 点对点路由
- 离线消息

### 难点
- 维护群成员在线状态
- 群消息高效路由

### 实现思路

1. 所有client链接到服务器后在数据表中记录自己所在的服务器ip
2. 假设client1发送消息给client2
3. 那么client1发送消息到自己链接的服务器1
4. 服务器1查找client2所在的服务器2
5. 服务器1把消息发送给服务总线（redis，rabbitmq）routing key为目标服务器ip
6. 所有服务器订阅服务总线，routing key是自己的ip
7. 目标服务器收到消息总线中订阅到的消息，把消息下发到client2
8. 如果目标服务器没有找到client2，那么可以相应的处理善后操作，比如持久消息


### 关于群聊的实现思路

> 难点：
> 消息分裂数量大，服务器过载大，资源浪费大
> 
> 谁可以帮帮我=。=

http://www.52im.net/thread-3687-1-1.html

1. 把用户分组合并
2. 不在线的群成员不投递信息，进入app再拉数据
3. todo

### 注册表
- 在线服务器注册到注册表
- 用户-服务器映射注册到注册表

### 定时任务
- 定期删除注册表中过期的数据

### signalr
>backplane需要实现`IMessageBus`接口

https://docs.microsoft.com/en-us/previous-versions/aspnet/jj908168(v=vs.100)


### netty相关

> 粘包拆包的内置实现
- FixedLengthFrameDecoder 按照特定长度组包
- DelimiterBasedFrameDecoder 按照指定分隔符组包, 例如本文中的$$$
- LineBasedFrameDecoder 按照换行符进行组包, \r \n等等