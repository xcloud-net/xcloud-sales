using JetBrains.Annotations;
using Volo.Abp.Application.Dtos;
using XCloud.Core.Helper;

namespace XCloud.Core.Dto;

public class MultipleErrorsDto : MultipleErrorsDto<object>
{
    //
}

public class MultipleErrorsDto<T> : IDataContainer<T>, IEntityDto
{
    public MultipleErrorsDto() { }
    public MultipleErrorsDto(T data) : this()
    {
        this.Data = data;
    }

    public virtual T Data { get; set; }

    IList<string> _errors;
    [NotNull]
    public IList<string> Errors
    {
        get
        {
            if (this._errors == null)
                this._errors = new List<string>();

            return this._errors;
        }
        set
        {
            this._errors = value;
        }
    }

    public bool HasErrors() => ValidateHelper.IsNotEmptyCollection(this.Errors);

    public string FirstErrorOrDefault() => this.Errors == null ? null : this.Errors.FirstOrDefault();

    public void AddError(string error)
    {
        if (string.IsNullOrWhiteSpace(error))
            throw new ArgumentNullException(nameof(AddError));

        this.Errors.Add(error);
    }
}

public class KeyDto<TKey> : IEntityDto
{
    public TKey Key { get; set; }
}

public class KeyValueDto<TKey, TValue> : IEntityDto
{
    public KeyValueDto() { }

    public KeyValueDto(TKey key, TValue value) : this()
    {
        this.Key = key;
        this.Value = value;
    }

    public TKey Key { get; set; }
    public TValue Value { get; set; }
}

public class IdDto<T> : IEntityDto<T>
{
    public IdDto() { }
    public IdDto(T id)
    {
        this.Id = id;
    }

    public T Id { get; set; }
}

public class IdDto : IEntityDto<string>
{
    public IdDto() { }
    public IdDto(string id)
    {
        Id = id;
    }

    public string Id { get; set; }

    public static implicit operator IdDto(string id) => new IdDto(id);

    public static implicit operator string(IdDto dto) => dto?.Id;
}

public class IdentityNameDto : IEntityDto
{
    public IdentityNameDto() { }
    public IdentityNameDto(string name)
    {
        IdentityName = name;
    }

    public string IdentityName { get; set; }

    public static implicit operator IdentityNameDto(string name) => new IdentityNameDto(name);

    public static implicit operator string(IdentityNameDto dto) => dto?.IdentityName;
}

public class NameDto : IEntityDto
{
    public string Name { get; set; }
}

public class IdNameDto : IdDto
{
    public string Name { get; set; }
}

public class SendSmsInput : IEntityDto
{
    public string Mobile { get; set; }
    public string CaptchaCode { get; set; }
}

public class SmsLoginInput : IEntityDto
{
    public string Mobile { get; set; }
    public string SmsCode { get; set; }
}

public class PasswordLoginDto : IEntityDto
{
    public string IdentityName { get; set; }
    public string Password { get; set; }
}

public class RefreshTokenDto : IEntityDto
{
    public string RefreshToken { get; set; }
}