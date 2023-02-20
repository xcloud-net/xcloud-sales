# auto mapper

> install AutoMapper.Extensions.Microsoft.DependencyInjection

## configuration

```csharp
builder.Services.AddAutoMapper(builder.AllModuleAssemblies);
```

## usage

```csharp
    public class AutoMapperProvider : IDataMapper
    {
        private readonly IMapper mapper;
        public AutoMapperProvider(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public TDestination Map<TSource, TDestination>(TSource source)
        {
            var res = this.mapper.Map<TSource, TDestination>(source);
            return res;
        }
    }
```

## register imapper

```csharp
    internal static class ValidatorBuilder
    {
        public static IXCloudBuilder AddFluentObjectValidator(this IXCloudBuilder builder)
        {
            builder.Services.RemoveAll<IDataValidatior>();
            builder.Services.AddTransient<IDataValidatior, FluentDataValidatior>();

            RegEntityValidators(builder, builder.AllModuleAssemblies);
            return builder;
        }

        static IXCloudBuilder RegEntityValidators(IXCloudBuilder builder, IEnumerable<Assembly> search_in_assembly)
        {
            var all_types = search_in_assembly.GetAllTypes().Where(x => x.IsNormalPublicClass()).ToArray();

            foreach (var type in all_types)
            {
                var validators = type.GetAllInterfaces_()
                    .Where(x => x.IsGenericType_(typeof(IValidator<>)))
                    .ToArray();

                //这里事实上只有一个
                foreach (var m in validators)
                {
                    builder.Services.RemoveAll(m);
                    builder.Services.AddTransient(m, type);
                }
            }
            return builder;
        }
    }
```