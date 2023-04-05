using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Auditing;
using XCloud.Core.Application.Entity;
using XCloud.Core.Dto;
using XCloud.Core.Extension;
using XCloud.Database.EntityFrameworkCore.ValueGenerators;

namespace XCloud.Database.EntityFrameworkCore.Mapping;

public static class MappingExtension
{
    public static void TryConfig<T>(this EntityTypeBuilder<T> builder)
        where T : class
    {
        var entityType = typeof(T);
        var configMethods = typeof(MappingExtension).GetPublicStaticMethods();

        void ExecuteConfig(string name)
        {
            var m = configMethods.FirstOrDefault(x => x.Name == name);

            if (m == null)
                throw new AbpException("ef mapping config method is not exist");

            m.MakeGenericMethod(typeof(T)).Invoke(obj: null, parameters: new object[] { builder });
        }

        var configProviders = new Dictionary<Type, string>()
        {
            [typeof(IHasAppFields)] = nameof(ConfigAppEntity),
            [typeof(IEntityBase)] = nameof(ConfigBaseEntity),
            [typeof(ISoftDelete)] = nameof(ConfigAbpSoftDeleteEntity),
            [typeof(IHasDeletionTime)] = nameof(ConfigAbpSoftDeletionTimeEntity),
            [typeof(IHasIdentityNameFields)] = nameof(ConfigIdentityNameEntity),
            [typeof(IHasCreationTime)] = nameof(ConfigAbpCreationTimeEntity),
            [typeof(ITreeEntityBase)] = nameof(ConfigTreeEntity),
            [typeof(IHasModificationTime)] = nameof(ConfigModificationTimeEntity)
        };

        var allEntityInterfaces = typeof(T).GetInterfaces().Distinct().ToArray();

        foreach (var provider in configProviders)
        {
            if (!entityType.IsAssignableTo(provider.Key))
            {
                continue;
            }

            var configMethod = provider.Value;
            ExecuteConfig(configMethod);
        }
    }

    public static void ConfigAppEntity<T>(this EntityTypeBuilder<T> builder)
        where T : class, IHasAppFields
    {
        builder.Property(x => x.AppKey).HasMaxLength(100);
    }

    public static void ConfigBaseEntity<T>(this EntityTypeBuilder<T> builder)
        where T : class, IEntityBase
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired()
            .HasMaxLength(100)
            .ValueGeneratedNever()
            .HasComment("主键，最好是顺序生成的GUID");

        //builder.HasIndex(x => x.Id).IsUnique();
    }

    public static void ConfigAbpSoftDeleteEntity<T>(this EntityTypeBuilder<T> builder)
        where T : class, ISoftDelete
    {
        builder.Property(x => x.IsDeleted).IsRequired().HasDefaultValue((int)YesOrNoEnum.No).HasComment("是否删除");
        builder.HasIndex(x => x.IsDeleted);
        builder.HasQueryFilter(x => x.IsDeleted == false);
    }

    public static void ConfigAbpCreationTimeEntity<T>(this EntityTypeBuilder<T> builder)
        where T : class, IHasCreationTime
    {
        builder.Property(x => x.CreationTime)
            .HasValueGenerator<UtcTimeGenerator>()
            .ValueGeneratedOnAdd()
            .HasComment("创建时间");

        //builder.HasIndex(x => x.Id).IsUnique();
        builder.HasIndex(x => x.CreationTime);
    }

    public static void ConfigAbpSoftDeletionTimeEntity<T>(this EntityTypeBuilder<T> builder)
        where T : class, IHasDeletionTime
    {
        builder.Property(x => x.DeletionTime).HasComment("删除时间");
    }

    public static void ConfigIdentityNameEntity<T>(this EntityTypeBuilder<T> builder)
        where T : class, IHasIdentityNameFields
    {
        builder.Property(x => x.IdentityName).IsRequired().HasMaxLength(100);
        builder.HasIndex(x => x.IdentityName).IsUnique();
    }

    public static void ConfigTreeEntity<T>(this EntityTypeBuilder<T> builder)
        where T : class, ITreeEntityBase
    {
        builder.Property(x => x.ParentId)
            .IsRequired()
            .HasMaxLength(100)
            .HasDefaultValue(string.Empty)
            .HasComment("树的父级节点id");

        builder.Property(x => x.TreeGroupKey).HasMaxLength(100);

        builder.HasIndex(x => x.ParentId);
    }

    public static void ConfigModificationTimeEntity<T>(this EntityTypeBuilder<T> builder)
        where T : class, IHasModificationTime
    {
        builder.Property(x => x.LastModificationTime)
            .HasComment("更新时间，同时也是乐观锁");

        builder.HasIndex(x => x.LastModificationTime);
    }
}