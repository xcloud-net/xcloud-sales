# abp permission
abp重新实现了iauthorizationservice
## 调用链路
- permission requirement handler
- permission checker
- permission value provider
- permission store （我们只要实现这个，同时可能要实现authorization policy provider）