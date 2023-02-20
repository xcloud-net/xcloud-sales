using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using XCloud.Core.DataSerializer.TextJson;
using XCloud.Core.Dto;
using XCloud.Core.Extension;
using XCloud.Platform.Core.Domain.User;

namespace XCloud.Test;

[TestClass]
public class CommonTest
{
    enum MyEnum
    {
        One = 1,
        Two = 2
    }

    [TestMethod]
    public void test_enum()
    {
        var e = (MyEnum)0;
        Console.WriteLine(e.ToString());
    }

    [TestMethod]
    public void linq_except()
    {
        var a = new[]
        {
            1, 2, 3, 4, 5, 6, 7,
            1, 2, 3, 4, 5, 6, 7,
            1, 2, 3, 4, 5, 6, 7
        };

        var b = new[] { 7 };

        var r1 = a.Except(b).ToArray();
        var r2 = a.ExceptBy(b, x => x).ToArray();
        var r22 = a.NotInBy(b, x => x).ToArray();

        b = new int[] { };
        var r3 = a.Except(b).ToArray();
        var r4 = a.ExceptBy(b, x => x).ToArray();
        var r44 = a.NotInBy(b, x => x).ToArray();
    }

    [TestMethod]
    public void dns_test()
    {
        var hostName = Dns.GetHostName();
        var address = Dns.GetHostAddresses(hostName);
    }

    [TestMethod]
    public void internallock_test()
    {
        int i = 0;
        Interlocked.Exchange(ref i, 1).Should().Be(0);
        i.Should().Be(1);

        Interlocked.Exchange(ref i, 2).Should().Be(1);
        i.Should().Be(2);
    }

    [TestMethod]
    public void Xxffddss()
    {
        var collection = new ServiceCollection();

        collection.Configure<SysUser>(option => option.NickName = "9");

        using var s = collection.BuildServiceProvider().CreateScope();

        s.ServiceProvider.GetService<SysUser>().Should().BeNull();

        var op = s.ServiceProvider.GetRequiredService<IOptions<SysUser>>().Value;
        op.Should().NotBeNull();

        (op.NickName == "9").Should().BeTrue();

        op.NickName = "10";

        (s.ServiceProvider.GetService<IOptions<SysUser>>().Value.NickName == "10").Should().BeTrue();
    }

    [TestMethod]
    public void test_cancellation_token_default()
    {
        (default(CancellationToken) == CancellationToken.None).Should().BeTrue();
    }

    public class SysUserExtend : SysUser
    {
        public bool Xxdd { get; set; }
    }

    [TestMethod]
    public void text_json_test()
    {
        var data = new SysUserExtend()
        {
            NickName = "xxdd",
            Xxdd = true
        };

        var json = System.Text.Json.JsonSerializer.Serialize(data, typeof(SysUser));
        var json2 = System.Text.Json.JsonSerializer.Serialize(data);
    }

    [TestMethod]
    public void json_test_xx()
    {
        var option = new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        };
        option.Converters.Add(new DateTimeJsonConverter());
        option.Converters.Add(new NullableDateTimeJsonConverter());

        var json = System.Text.Json.JsonSerializer.Serialize(new SysUser() { NickName = "x", CreationTime = DateTime.Now }, option);

        var data = JsonSerializer.Deserialize<SysUser>(json, option);

        json = System.Text.Json.JsonSerializer.Serialize(new { name = "x", time = 123 }, option);
        data = JsonSerializer.Deserialize<SysUser>(json, option);

        using var doc = JsonDocument.Parse(json, new JsonDocumentOptions() { });
        var prop = doc.RootElement.EnumerateObject().FirstOrDefault(x => x.Name == "name");
        prop.Should().NotBeNull();
        (prop.Value.ValueKind == JsonValueKind.String).Should().BeTrue();
        prop.Value.GetString().Should().Be("x");
    }

    class Attr1Attribute : Attribute
    {
    }

    class Attr2Attribute : Attribute
    {
    }

    abstract class A
    {
        [Attr1]
        public virtual void Test()
        {
        }
    }

    class B : A
    {
        [Attr2]
        public override void Test()
        {
            base.Test();
        }
    }

    [TestMethod]
    public void attr_inherit_test()
    {
        var m = typeof(B).GetMethods().FirstOrDefault(x => x.Name == "test");
        m.Should().NotBeNull();
        m.GetCustomAttributes(inherit: true).Length.Should().Be(2);
        m.GetCustomAttributes(inherit: false).Length.Should().Be(1);
    }

    [TestMethod]
    public void operator_test____()
    {
        var user = new SysUser();

        object.Equals(user, null).Should().BeFalse();
        object.Equals(null, user).Should().BeFalse();

        object.Equals(user, new SysUser()).Should().BeFalse();

        user.Id = ("1");
        object.Equals(user, new SysUser("1")).Should().BeTrue();

        (user == new SysUser("1")).Should().BeTrue();

        (user is null).Should().BeFalse();
    }

    [TestMethod]
    public void json_test()
    {
        var obj = JObject.Parse("{\"name\":\"wj\"}");
        var field = obj["name"];

        var value = field.Value<string>();
        value.Should().Be("wj");
    }

    [TestMethod]
    public void compare_test()
    {
        int a = 1, b = 3;

        new Action(() => a.CompareTo(b)).Should().NotThrow();
        new Action(() => a.CompareTo(new { })).Should().Throw<Exception>();
        new Action(() => a.CompareTo(1f)).Should().Throw<Exception>();

        object c = b;
        new Action(() => a.CompareTo(c)).Should().NotThrow();
    }

    [TestMethod]
    public void enum_test()
    {
        var id = (int)CrudEnum.Delete;

        var e = (CrudEnum)id;
        e.ToString().Should().Be("Delete");

        ((CrudEnum)4564).ToString().Should().Be("4564");
    }

    [TestMethod]
    public void timezone_test()
    {
        var allTimezone = TimeZoneInfo.GetSystemTimeZones().ToArray();

        var timezone = TimeZoneInfo.CreateCustomTimeZone("中国/上海东八区", TimeSpan.FromHours(8), "中国东八区", "中国东八区");

        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("zh");
        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("zh");
        var allCultrues = CultureInfo.GetCultures(CultureTypes.AllCultures);
    }

    [TestMethod]
    public void time_zone_test()
    {
        var now = DateTime.Now;
        now.Kind.Should().Be(DateTimeKind.Local);
        var utcNow = now.ToUniversalTime();
        utcNow.Kind.Should().Be(DateTimeKind.Utc);

        //这个是代码运行机器的时区
        var zone = TimeZoneInfo.Local;
        var convertedLocalTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, zone);
        now.Should().Be(convertedLocalTime);

        var utcTimeStr = utcNow.ToString();
        var timeStr = "2018-01-25 15:23:44";

        var signTime = DateTime.Parse(timeStr);
        signTime.Kind.Should().Be(DateTimeKind.Unspecified);
        var dateTimeUtc = DateTime.SpecifyKind(signTime, DateTimeKind.Utc);
        dateTimeUtc.Kind.Should().Be(DateTimeKind.Utc);

        var timeoffset = DateTimeOffset.Parse(timeStr);
        //

        var utcTime = DateTime.SpecifyKind(signTime, DateTimeKind.Utc);
    }

    [TestMethod]
    public void nullable_test()
    {
        Nullable<int> a = 1;
        (a is int).Should().BeTrue();

        a = null;
        (a is int).Should().BeFalse();
    }

    [TestMethod]
    public async Task InvokeMethod___()
    {
        var obj = new CommonTest();
        var methods = obj.GetType()
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        bool IsAsync(Type t)
        {
            return t.IsAssignableTo_<Task>();
        }

        async Task Invoke(string name)
        {
            var m = methods.FirstOrDefault(x => x.Name == name);
            var res = m.Invoke(obj, new object[] { });
            if (IsAsync(m.ReturnType))
            {
                var task = (Task)res;
                await task;
            }
        }

        await Invoke(nameof(xx_1));
        await Invoke(nameof(xx_2));
        await Invoke(nameof(xx_3));
    }

    void xx_1() => Console.WriteLine(string.Empty);

    async Task<string> xx_2() => await Task.FromResult(string.Empty);

    async Task xx_3() => await Task.FromResult(string.Empty);

    /// <summary>
    /// todo 通过il生成cap consumer
    /// </summary>
    [TestMethod]
    public void Testxxxxdddd()
    {
        var assBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("xxdd"), AssemblyBuilderAccess.Run);
        var moduleBuilder = assBuilder.DefineDynamicModule("kjljkl");

        var typeBuilder = moduleBuilder.DefineType("fa", TypeAttributes.Public | TypeAttributes.Class);
        var c = typeBuilder.DefineConstructor(
            MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName |
            MethodAttributes.RTSpecialName, CallingConventions.Standard,
            parameterTypes: new Type[] { });
    }
}