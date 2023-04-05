namespace XCloud.Core.Application.Entity;

public interface IHasAppFields
{
    string AppKey { get; set; }
}

public interface IHasIdentityNameFields
{
    string IdentityName { get; set; }
    string OriginIdentityName { get; set; }
}