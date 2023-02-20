# 授权服务器

### 如何签发ids证书

``` shell
openssl req -newkey rsa:2048 -nodes -keyout ./xx.key -x509 -days 3650 -out ./xx.cer
```

``` shell
openssl pkcs12 -export -in ./xx.cer -inkey ./xx.key -out ./ids4.pfx
```