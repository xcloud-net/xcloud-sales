# mall-ui 前端项目

## Getting Started

> 我不是很会前端开发，希望有专业人士可以牵头此项目前端开发。
> 有意愿的可以和我联系。

`此项目基于node 16.18.1开发测试`

⚠️因为组件是dynamic load，所以前后台都放在一个项目中。
首次加载速度尚可接受。

### install

```bash
$ npm install
```

### dev
```bash
$ export gatewayAddress=https://api.your-domain.com:8888 && npm run dev
```

### build

```bash
# build mobile-app and management-app
$ export gatewayAddress=https://api.your-domain.com:8888 && npm run build
# build mobile-app only [smaller size,without management app]
$ export gatewayAddress=https://api.your-domain.com:8888 && npm run build-app-only
```
