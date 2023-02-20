# clients

- sales项目本来是不直接依赖platform的，而是通过api来通讯。
- 但是目前由于开发繁杂度比较高，所以改为直接依赖
- 所有sales项目调用platform都需要在这个client中桥接
- 不限于platform，sales接入其他任何系统都要在这个clients桥接