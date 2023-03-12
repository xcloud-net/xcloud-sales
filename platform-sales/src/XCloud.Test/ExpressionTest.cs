using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Volo.Abp;
using XCloud.Core.Application;
using XCloud.Core.Application.Entity;
using XCloud.Platform.Core.Domain.User;

namespace XCloud.Test;

[TestClass]
public class ExpressionTest
{
    [TestMethod]
    public void TestMemberExpression()
    {
        var testUserName = "test-user-name";
        var entity = new SysUser() { IdentityName = testUserName };

        var entityType = entity.GetType();
        //x
        var parameterExpr = Expression.Parameter(entityType, "x");

        var userNameProperty = entityType.GetProperties()
            .FirstOrDefault(x => x.Name == nameof(entity.IdentityName));

        if (userNameProperty == null)
            throw new AbpException(nameof(userNameProperty));

        if (userNameProperty.PropertyType != typeof(string))
            throw new AbpException("wrong creation time type");

        //x.Id
        var fieldSelectorExpr =
            Expression.Property(expression: parameterExpr, property: userNameProperty);

        var expression =
            Expression.Lambda<Func<SysUser, string>>(fieldSelectorExpr,
                new ParameterExpression[] { parameterExpr });

        var userName = expression.Compile().Invoke(entity);

        (userName == testUserName).Should().BeTrue();
    }

    [TestMethod]
    public void test_ref()
    {
        var q = new[] { new SysUser() }.AsQueryable();

        var anotherQ = q.Where(x => x.NickName.StartsWith("x"));
        object.ReferenceEquals(q, anotherQ).Should().BeFalse();
    }

    [TestMethod]
    public void TestSetFields()
    {
        var c = new SysUser();

        c = c.SetFields(x => x.IdentityName, "xx");
        (c.IdentityName == "xx").Should().BeTrue();
    }

    private Expression<Func<T, object>> OrderByExpressionConvert<T, SortType>(Expression<Func<T, SortType>> field)
    {
        if (field.Body is MemberExpression exp)
        {
            //参数
            var parameterType = field.Parameters.FirstOrDefault()?.Type;
            parameterType.Should().NotBeNull();
            ParameterExpression parameter = Expression.Parameter(parameterType, "x");
            //access member
            var property = exp.Member as PropertyInfo;
            property.Should().NotBeNull();
            MemberExpression body = Expression.Property(parameter, property: property);

            Expression<Func<T, object>> res =
                Expression.Lambda<Func<T, object>>(body: body, parameters: new[] { parameter });

            return res;
        }
        else
        {
            throw new NotSupportedException(nameof(OrderByExpressionConvert));
        }
    }

    [TestMethod]
    public void TestMemberExpressionConvert()
    {
        Expression<Func<SysUser, string>> ex = x => x.NickName;

        var x = this.OrderByExpressionConvert(ex);

        var res = x.Compile()(new SysUser() { NickName = "blue" });

        (res is string a && a == "blue").Should().BeTrue();
    }

    [TestMethod]
    public void expression_new_obj()
    {
        Expression<Func<SysUser, object>> expression = x => new { x.IdentityName, x.Id };

        var param = expression.Parameters.First();
        var body = (System.Linq.Expressions.NewExpression)expression.Body;

        //body.Members

        var nameField = (MemberExpression)body.Arguments.First();
        var nameFieldParam = (ParameterExpression)nameField.Expression;

        (nameFieldParam == param).Should().BeTrue();
    }

    [TestMethod]
    public void method_generator()
    {
        var t = typeof(string);
        var m = t.GetMethods().FirstOrDefault(x => x.Name == "ToUpper");
        "".ToUpper();
        var p = Expression.Parameter(t, "x");
        var call = Expression.Call(p, m);
        Expression<Action<string>> exp = Expression.Lambda<Action<string>>(call, p);
        var d = Expression.Lambda(typeof(Action<string>), call, p);

        var sd = d as Expression<Action<string>>;
    }

    [TestMethod]
    public void member_access_expression_test()
    {
        Expression<Func<SysUser, string>> exp = x => x.IdentityName;
        var p = ((exp.Body as MemberExpression).Member as PropertyInfo);

        var u = new SysUser();

        p.SetValue(u, "wj");

        u.IdentityName.Should().Be("wj");
    }
}