using Volo.Abp.Data;
using Volo.Abp.Http;

namespace XCloud.Core.Dto;

public interface IDataContainer<T>
{
    T Data { get; set; }
}

/// <summary>
/// 接口公共返回值的缩写
/// </summary>
public class ApiResponse<T> : RemoteServiceErrorResponse, IDataContainer<T>, IHasExtraProperties
{
    public T Data { get; set; }

    ExtraPropertyDictionary _extraProperties;

    public ExtraPropertyDictionary ExtraProperties
    {
        get
        {
            if (this._extraProperties == null)
                this._extraProperties = new ExtraPropertyDictionary();

            return this._extraProperties;
        }
    }

    /// <summary>
    /// 默认为success，error为null
    /// </summary>
    public ApiResponse() : base(null)
    {
        //
    }

    public ApiResponse(T data) : base(null)
    {
        this.Data = data;
    }

    public static implicit operator bool(ApiResponse<T> data) => data.Error == null;
}