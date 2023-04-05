using System.Linq.Expressions;
using System.Reflection;
using FluentAssertions;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace XCloud.Core.Application.Entity;

public static class EntityExtension
{
    public static IEnumerable<T> Ids<T>(this IEnumerable<IEntity<T>> list) => list.Select(x => x.Id);

    public static T SetEntityFields<T>(this T model, object data, Func<PropertyInfo, bool> ignorePropertyFunc = null)
        where T : class, IEntity
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        if (data == null)
            throw new ArgumentNullException(nameof(data));

        ignorePropertyFunc ??= x => false;

        var props = model.GetType().GetProperties();

        foreach (var m in data.GetType().GetProperties())
        {
            if (ignorePropertyFunc.Invoke(m))
                continue;

            var val = m.GetValue(data);

            var prop = props.FirstOrDefault(x => x.Name == m.Name);

            if (prop == null)
                throw new AbpException("set data,field not found");

            if (!prop.CanWrite)
                throw new AbpException("set data,field can't be written");

            prop.SetValue(model, val);
        }

        return model;
    }

    public static T SetFields<T, F>(this T model, Expression<Func<T, F>> field, F value) where T : class, IEntity
    {
        field.Should().NotBeNull();

        if (field.Body.NodeType == ExpressionType.MemberAccess && field.Body is MemberExpression member)
        {
            if (member.Member is PropertyInfo prop)
            {
                prop.CanWrite.Should().BeTrue("字段不可以写入");
                prop.SetValue(model, value);
                return model;
            }
        }

        throw new NotSupportedException($"{nameof(SetFields)}");
    }
}