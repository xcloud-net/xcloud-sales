namespace XCloud.Platform.Application.Common.Service.AsyncJob;

public enum JobStatusEnum : int
{
    未开始 = 0,
    正在运行 = 1,
    处理成功 = 2,
    处理失败 = 3
}