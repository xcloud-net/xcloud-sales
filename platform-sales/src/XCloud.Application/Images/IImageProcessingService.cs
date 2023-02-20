using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SkiaSharp;
using Volo.Abp.DependencyInjection;

namespace XCloud.Application.Images;

public interface IImageProcessingService
{
    Task<byte[]> ResizeAsync(byte[] bs, int width, int height);
}

[ExposeServices(typeof(IImageProcessingService))]
public class ImageSharpImageProcessingService : IImageProcessingService, IScopedDependency
{
    private readonly ILogger _logger;

    public ImageSharpImageProcessingService(ILogger<ImageSharpImageProcessingService> logger)
    {
        this._logger = logger;
    }

    /// <summary>
    /// https://www.javaroad.cn/questions/294295
    /// 
    /// //bug,todo
    /// 
    /// 我在Xamarin.Forms项目中使用SkiaSharp中的SKBitmap.Resize（）方法来调整图像大小以供显示 . 
    /// 我遇到的问题是在iOS上拍照时，当照片以肖像拍摄时，图像会以正面朝上显示 . 
    /// 在Android上拍照，从Android和iOS设备上的照片库导入可以保持方向，但在iOS上拍照不会 . 
    /// 如果我没有使用SkiaSharp调整图像大小（仅显示图像而不调整大小），则图像将以正确的方向显示 . 
    /// 然而，这不是解决方案，因为图像需要调整大小 . 以下是我的代码 -
    /// </summary>
    public async Task<byte[]> ResizeAsync(byte[] bs, int width, int height)
    {
        if (width <= 0 && height <= 0)
            return bs;

        using var img = SKBitmap.Decode(bs);

        width = Math.Min(width, img.Width);
        height = Math.Min(height, img.Height);

        if (width <= 0 || height <= 0)
        {
            if (width <= 0)
            {
                var rate = (height * 1.0) / (img.Height * 1.0);
                width = (int)(rate * img.Width);
            }

            if (height <= 0)
            {
                var rate = (width * 1.0) / (img.Width * 1.0);
                height = (int)(rate * img.Height);
            }
        }

        if (width == img.Width && height == img.Height)
        {
            return bs;
        }

        try
        {
            using var resizedBitmap = img.Resize(new SKImageInfo((int)width, (int)height), SKFilterQuality.Medium);
            using var cropImage = SKImage.FromBitmap(resizedBitmap);

            //In order to exclude saving pictures in low quality at the time of installation, we will set the value of this parameter to 80 (as by default)
            using var data = cropImage.Encode(SKEncodedImageFormat.Png, quality: 80);

            var resizedBytes = data.ToArray();
            return resizedBytes;
        }
        catch (Exception e)
        {
            this._logger.LogWarning(message: e.Message, exception: e);

            await Task.CompletedTask;
            return bs;
        }
    }
}