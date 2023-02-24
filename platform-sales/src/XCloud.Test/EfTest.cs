using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using FluentAssertions;
using Volo.Abp.Domain.Entities;
using XCloud.Platform.Core.Domain.User;

namespace XCloud.Test;

[TestClass]
public class EfTest
{
    public class DbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(databaseName: "test-db");
        }

        public DbSet<SysUser> SysUser { get; set; }
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
            (db.SysUser == db.Set<SysUser>()).Should().BeTrue();

            var model = new SysUser()
            {
                IdentityName = DateTime.Now.ToString(),
                CreationTime = DateTime.UtcNow
            };
            model.Id = ("123");

            (db.Entry(model).State == EntityState.Detached).Should().BeTrue();

            db.Set<SysUser>().Attach(model);
            (db.Entry(model).State == EntityState.Unchanged).Should().BeTrue();

            model.IdentityName = "fa";
            (db.Entry(model).State == EntityState.Modified).Should().BeTrue();

            db.Set<SysUser>().Remove(model);
            (db.Entry(model).State == EntityState.Deleted).Should().BeTrue();

            db.SysUser.Add(model);
            (db.Entry(model).State == EntityState.Added).Should().BeTrue();
            db.SaveChanges();
            (db.Entry(model).State == EntityState.Unchanged).Should().BeTrue();

            model.IdentityName = "xx";
            (db.Entry(model).State == EntityState.Modified).Should().BeTrue();
            db.SaveChanges();
            (db.Entry(model).State == EntityState.Unchanged).Should().BeTrue();

            var newUser = db.Set<SysUser>().AsNoTracking().FirstOrDefault(x => x.IdentityName == "xx");

            if (newUser == null)
                throw new EntityNotFoundException(nameof(newUser));

            (db.Entry(newUser).State == EntityState.Detached).Should().BeTrue();

            newUser = db.Set<SysUser>().AsTracking().FirstOrDefault(x => x.IdentityName == "xx");

            if (newUser == null)
                throw new EntityNotFoundException(nameof(newUser));

            (db.Entry(newUser).State == EntityState.Unchanged).Should().BeTrue();

            db.SysUser.Remove(model);
            (db.Entry(model).State == EntityState.Deleted).Should().BeTrue();
            db.SaveChanges();
            (db.Entry(model).State == EntityState.Detached).Should().BeTrue();
        }
    }
}