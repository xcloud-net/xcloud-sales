# 身份认证相关

> 解析 token，生成 claims

- cookie authentication
- jwt authentication
- third party oauth authentication providers
- 提供通过 claims 查询详细用户信息的 service(web api)

# 过程
token(ref token或者jwt)->authentication->claim based information->check database->authorization(permission check)