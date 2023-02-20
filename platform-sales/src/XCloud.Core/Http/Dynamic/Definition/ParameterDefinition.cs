using System.Reflection;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace XCloud.Core.Http.Dynamic.Definition;

public class ParameterDefinition
{
    public ParameterInfo ParameterInfo { get; }
    public IBindingSourceMetadata BindingSourceMetadata { get; }
    public IBinderTypeProviderMetadata BinderTypeProviderMetadata { get; }

    public BindingSource BindingSource => this.BindingSourceMetadata?.BindingSource;

    public ParameterDefinition(ParameterInfo m)
    {
        m.Should().NotBeNull();
        this.ParameterInfo = m;

        var attrs = m.GetCustomAttributes();
        this.BindingSourceMetadata = attrs.OfType<IBindingSourceMetadata>().FirstOrDefault();
        this.BinderTypeProviderMetadata = attrs.OfType<IBinderTypeProviderMetadata>().FirstOrDefault();
    }

    public bool IsValidated()
    {
        if (this.BindingSource == null)
        {
            return false;
        }

        return true;
    }
}