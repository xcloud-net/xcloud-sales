using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Volo.Abp;
using XCloud.Core.Extension;

namespace XCloud.Application.Service;

public static class CrudHelper
{
    public static bool IsEmptyString(string str) => string.IsNullOrWhiteSpace(str);

    public static bool IsEmptyInt(int i) => i <= 0;

    public static bool IsEmptyLong(long i) => i <= 0;

    public static bool IsEmptyKey<TKey>(TKey id)
    {
        var keyType = typeof(TKey);
        var methods = typeof(CrudHelper).GetPublicStaticMethods();

        MethodInfo GetMethodOrThrow(string name)
        {
            var m = methods.FirstOrDefault(x => x.Name == name);
            if (m == null)
                throw new NotSupportedException(name);
            return m;
        }

        bool CastToBool([CanBeNull] object result)
        {
            if (result == null)
                throw new AbpException("wrong result");
            return (bool)result;
        }

        if (keyType == typeof(string))
        {
            var m = GetMethodOrThrow(nameof(CrudHelper.IsEmptyString));
            var result = m.Invoke(obj: null, parameters: new object[] { id });
            return CastToBool(result);
        }

        if (keyType == typeof(int))
        {
            var m = GetMethodOrThrow(nameof(CrudHelper.IsEmptyInt));
            var result = m.Invoke(obj: null, parameters: new object[] { id });
            return CastToBool(result);
        }

        if (keyType == typeof(long))
        {
            var m = GetMethodOrThrow(nameof(CrudHelper.IsEmptyLong));
            var result = m.Invoke(obj: null, parameters: new object[] { id });
            return CastToBool(result);
        }

        throw new NotSupportedException(nameof(IsEmptyKey));
    }
}