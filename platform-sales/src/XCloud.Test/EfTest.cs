using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using FluentAssertions;
using XCloud.Core.Application;

namespace XCloud.Test;

[TestClass]
public class EfTest
{
    [Table(nameof(UserTest))]
    public class UserTest : EntityBase
    {
        public string Name { get; set; }
    }

    public class DbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var constr = "Server=mysql;Database=test-ef-core-01;Uid=root;Pwd=123;port=3306;CharSet=utf8";
            var serverVersion = ServerVersion.AutoDetect(constr);
            optionsBuilder.UseMySql(constr, serverVersion);
        }

        public DbSet<UserTest> UserTest { get; set; }
    }

    [TestInitialize]
    public void test_init()
    {
        using var db = new DbContext();
        db.Database.EnsureCreated();
    }

    [TestCleanup]
    public void test_cleanup()
    {
        using var db = new DbContext();
        db.Database.EnsureDeleted();
    }

    [TestMethod]
    public void repo_test()
    {
        using (var db = new DbContext())
        {
            (db.UserTest == db.Set<UserTest>()).Should().BeTrue();

            var model = new UserTest()
            {
                Name = DateTime.Now.ToString(),
                CreationTime = DateTime.UtcNow
            };
            model.Id = ("123");

            (db.Entry(model).State == EntityState.Detached).Should().BeTrue();

            db.Set<UserTest>().Attach(model);
            (db.Entry(model).State == EntityState.Unchanged).Should().BeTrue();

            model.Name = "fa";
            (db.Entry(model).State == EntityState.Modified).Should().BeTrue();

            db.Set<UserTest>().Remove(model);
            (db.Entry(model).State == EntityState.Deleted).Should().BeTrue();

            db.UserTest.Add(model);
            (db.Entry(model).State == EntityState.Added).Should().BeTrue();
            db.SaveChanges();
            (db.Entry(model).State == EntityState.Unchanged).Should().BeTrue();

            model.Name = "xx";
            (db.Entry(model).State == EntityState.Modified).Should().BeTrue();
            db.SaveChanges();
            (db.Entry(model).State == EntityState.Unchanged).Should().BeTrue();

            {
                var newUser = db.Set<UserTest>().AsNoTracking().FirstOrDefault(x => x.Name == "xx");
                (db.Entry(newUser).State == EntityState.Detached).Should().BeTrue();

                newUser = db.Set<UserTest>().AsTracking().FirstOrDefault(x => x.Name == "xx");
                (db.Entry(newUser).State == EntityState.Unchanged).Should().BeTrue();
            }

            db.UserTest.Remove(model);
            (db.Entry(model).State == EntityState.Deleted).Should().BeTrue();
            db.SaveChanges();
            (db.Entry(model).State == EntityState.Detached).Should().BeTrue();
        }
    }
}