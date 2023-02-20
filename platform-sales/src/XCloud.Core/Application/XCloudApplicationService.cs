﻿using Microsoft.Extensions.Configuration;
using Volo.Abp.Application.Services;
using Volo.Abp.Validation;
using XCloud.Core.Cache;
using XCloud.Core.DataSerializer;

namespace XCloud.Core.Application;

public interface IXCloudApplicationService : IApplicationService
{
    //
}

public abstract class XCloudApplicationService : ApplicationService
{
    protected IConfiguration Configuration => this.LazyServiceProvider.LazyGetRequiredService<IConfiguration>();

    protected ICacheProvider CacheProvider => this.LazyServiceProvider.LazyGetRequiredService<ICacheProvider>();

    protected IJsonDataSerializer JsonDataSerializer =>
        this.LazyServiceProvider.LazyGetRequiredService<IJsonDataSerializer>();

    protected IObjectValidator ObjectValidator =>
        this.LazyServiceProvider.LazyGetRequiredService<IObjectValidator>();
}