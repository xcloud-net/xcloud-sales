using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.Validation;
using XCloud.Core.Extension;

namespace XCloud.Application.Validation;

public class NullMethodInvocationValidator : IMethodInvocationValidator
{
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger logger;
    public NullMethodInvocationValidator(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
        logger = serviceProvider.ResolveLogger<NullMethodInvocationValidator>();
    }

    public Task ValidateAsync(MethodInvocationValidationContext context)
    {
        return Task.CompletedTask;
    }
}