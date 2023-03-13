using System.IO;
using System.Threading.Tasks;
using COSXML.Model.Object;

namespace XCloud.Platform.Application.Common.Service.Storage.QCloud;

public static class CosServerWarpObjectExtensions
{
    public static async Task<bool> CheckObjectIsExistAsync(this CosServerWrapObject client, string bucketName, string objectKey)
    {
        var request = new DoesObjectExistRequest(bucketName, objectKey);
        var exist = client.CosXmlServer.DoesObjectExist(request);

        await Task.CompletedTask;

        return exist;
    }

    public static async Task UploadObjectAsync(this CosServerWrapObject client,
        string bucketName,
        string objectKey,
        Stream data)
    {
        var bs = data.GetAllBytes();
        var request = new PutObjectRequest(bucketName, objectKey, bs);

        var response = client.CosXmlServer.PutObject(request);

        if (!response.IsSuccessful())
        {
            await Task.CompletedTask;
            throw new BusinessException(nameof(UploadObjectAsync)).WithData("qcloud-cos", response.GetResultInfo());
        }
    }

    public static async Task<Stream> DownloadObjectOrNullAsync(this CosServerWrapObject client,
        string bucketName,
        string objectKey)
    {
        var request = new GetObjectBytesRequest(bucketName, objectKey);

        var response = client.CosXmlServer.GetObject(request);

        if (!response.IsSuccessful())
        {
            await Task.CompletedTask;
            return null;
        }

        return new MemoryStream(response.content);
    }

    public static async Task DeleteObjectAsync(this CosServerWrapObject client,
        string bucketName,
        string objectKey)
    {
        var request = new DeleteObjectRequest(bucketName, objectKey);
        var response = client.CosXmlServer.DeleteObject(request);

        if (!response.IsSuccessful())
        {
            await Task.CompletedTask;
            throw new BusinessException(nameof(DeleteObjectAsync)).WithData("qcloud-cos", response.GetResultInfo());
        }
    }
}