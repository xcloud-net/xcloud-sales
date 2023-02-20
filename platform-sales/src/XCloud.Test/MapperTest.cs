using AutoMapper;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using Volo.Abp.Application.Dtos;
using XCloud.Core.Application;

namespace XCloud.Test;

[TestClass]
public class MapperTest
{
    class AEntity : EntityBase
    {
    }

    class ADto : IEntityDto<string>
    {
        public string Id { get; set; }
    }

    class AMapper : Profile
    {
        public AMapper()
        {
            this.CreateMap<AEntity, ADto>().ForMember(x => x.Id, f => f.MapFrom(x => x.Id)).ReverseMap();
        }
    }

    [TestMethod]
    public void test_mapper()
    {
        var dto = new ADto() { Id = "xx" };
        var config = new MapperConfiguration(cfg => { cfg.AddProfile<AMapper>(); });
        IMapper mapper = config.CreateMapper();
        var entity = mapper.Map<AEntity>(dto);
        entity.Id.Should().Be("xx");

        entity.Id = ("dd");
        dto = mapper.Map<ADto>(entity);
        dto.Id.Should().Be("dd");
    }

    [TestMethod]
    public void test_reflection()
    {
        //反射读取类私有属性
        Person per = new Person("ismallboy", "20102100104");
        Type t = per.GetType();
        //获取私有方法
        MethodInfo method = t.GetMethod("GetStuInfo", BindingFlags.NonPublic | BindingFlags.Instance);
        //访问无参数私有方法
        string strTest = method.Invoke(per, null).ToString();
        //访问有参数私有方法
        MethodInfo method2 = t.GetMethod("GetValue", BindingFlags.NonPublic | BindingFlags.Instance);
        object[] par = new object[2];
        par[0] = "ismallboy";
        par[1] = 2;
        string strTest2 = method2.Invoke(per, par).ToString();

        //获取私有字段
        PropertyInfo field = t.GetProperty("Name", BindingFlags.NonPublic | BindingFlags.Instance);
        //访问私有字段值
        string value = field.GetValue(per).ToString();
        //设置私有字段值
        field.SetValue(per, "new Name");
        value = field.GetValue(per).ToString();
    }

    /// <summary>
    /// 个人信息
    /// </summary>
    class Person
    {
        private string Name { get; set; }
        private string StuNo { get; set; }

        public Person(string name, string stuNo)
        {
            this.Name = name;
            this.StuNo = stuNo;
        }

        private string GetStuInfo()
        {
            return this.Name;
        }

        private string GetValue(string str, int n)
        {
            return str + n.ToString();
        }
    }
}