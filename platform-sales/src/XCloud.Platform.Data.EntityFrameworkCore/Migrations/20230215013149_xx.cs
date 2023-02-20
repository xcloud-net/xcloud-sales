using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XCloud.Platform.Data.EntityFrameworkCore.Migrations
{
    public partial class xx : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sys_activity_log",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "主键，最好是顺序生成的GUID"),
                    SubjectId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    SubjectType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    LogType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    AppId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ActionName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Log = table.Column<string>(type: "TEXT", nullable: true),
                    ExceptionDetail = table.Column<string>(type: "TEXT", nullable: true),
                    Data = table.Column<string>(type: "TEXT", nullable: true),
                    LogTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_activity_log", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sys_admin",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "主键，最好是顺序生成的GUID"),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsSuperAdmin = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_admin", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sys_admin_role",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "主键，最好是顺序生成的GUID"),
                    RoleId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    AdminId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_admin_role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sys_app",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "主键，最好是顺序生成的GUID"),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    AppKey = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Enabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_app", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sys_app_scope",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "主键，最好是顺序生成的GUID"),
                    SubjectId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SubjectType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    AppKey = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_app_scope", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sys_department",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "主键，最好是顺序生成的GUID"),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    LogoUrl = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false, comment: "是否删除"),
                    DeletionTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "删除时间"),
                    LastModificationTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "更新时间，同时也是乐观锁"),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间"),
                    ParentId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, defaultValue: "", comment: "树的父级节点id"),
                    TreeGroupKey = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_department", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sys_department_assignment",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "主键，最好是顺序生成的GUID"),
                    AdminId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DepartmentId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    IsManager = table.Column<int>(type: "INTEGER", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_department_assignment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sys_external_access_token",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "主键，最好是顺序生成的GUID"),
                    Platform = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    AppId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Scope = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    GrantType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    AccessTokenType = table.Column<int>(type: "INTEGER", nullable: false),
                    AccessToken = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    RefreshToken = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    ExpiredAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_external_access_token", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sys_external_connect",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "主键，最好是顺序生成的GUID"),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Platform = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    AppId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    OpenId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_external_connect", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sys_job",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "主键，最好是顺序生成的GUID"),
                    JobKey = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Desc = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    ExceptionMessage = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    ExtraData = table.Column<string>(type: "TEXT", nullable: true),
                    StartTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EndTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_job", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sys_menu",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "主键，最好是顺序生成的GUID"),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Url = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    ImageUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IconCls = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IconUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Sort = table.Column<int>(type: "INTEGER", nullable: false),
                    PermissionJson = table.Column<string>(type: "TEXT", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "更新时间，同时也是乐观锁"),
                    AppKey = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间"),
                    ParentId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, defaultValue: "", comment: "树的父级节点id"),
                    TreeGroupKey = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_menu", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sys_notification",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "主键，最好是顺序生成的GUID"),
                    Title = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Content = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    App = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Read = table.Column<bool>(type: "INTEGER", nullable: false),
                    ReadTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: true),
                    ActionType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Data = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    SenderId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    SenderType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_notification", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sys_permission",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "主键，最好是顺序生成的GUID"),
                    PermissionKey = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Group = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    AppKey = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_permission", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sys_region",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "主键，最好是顺序生成的GUID"),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Data = table.Column<string>(type: "TEXT", nullable: true),
                    RegionType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Sort = table.Column<int>(type: "INTEGER", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间"),
                    ParentId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, defaultValue: "", comment: "树的父级节点id"),
                    TreeGroupKey = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_region", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sys_resource_acl",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "主键，最好是顺序生成的GUID"),
                    ResourceType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ResourceId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PermissionType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PermissionId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    AccessControlType = table.Column<int>(type: "INTEGER", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_resource_acl", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sys_role",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "主键，最好是顺序生成的GUID"),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sys_role_permission",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "主键，最好是顺序生成的GUID"),
                    PermissionKey = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    RoleId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_role_permission", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sys_sequence",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "主键，最好是顺序生成的GUID"),
                    Category = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    NextId = table.Column<int>(type: "INTEGER", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "更新时间，同时也是乐观锁"),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_sequence", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sys_settings",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "主键，最好是顺序生成的GUID"),
                    Name = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sys_storage_meta",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "主键，最好是顺序生成的GUID"),
                    FileExtension = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    ContentType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ResourceSize = table.Column<long>(type: "INTEGER", nullable: false),
                    ResourceKey = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ResourceHash = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    HashType = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    ExtraData = table.Column<string>(type: "TEXT", nullable: true),
                    StorageProvider = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ReferenceCount = table.Column<int>(type: "INTEGER", nullable: false),
                    UploadTimes = table.Column<int>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false, comment: "是否删除"),
                    LastModificationTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "更新时间，同时也是乐观锁"),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_storage_meta", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sys_user",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "主键，最好是顺序生成的GUID"),
                    IdentityName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    OriginIdentityName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    NickName = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    PassWord = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Avatar = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    LastPasswordUpdateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Gender = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false, comment: "是否删除"),
                    DeletionTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "删除时间"),
                    LastModificationTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "更新时间，同时也是乐观锁"),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_user", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sys_user_address",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "主键，最好是顺序生成的GUID"),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Lat = table.Column<double>(type: "REAL", nullable: true),
                    Lng = table.Column<double>(type: "REAL", nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    NationCode = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Nation = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ProvinceCode = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Province = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CityCode = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    City = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    AreaCode = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Area = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    AddressDescription = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    AddressDetail = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    PostalCode = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Tel = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    DeletionTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "删除时间"),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false, comment: "是否删除"),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_user_address", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sys_user_identity",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "主键，最好是顺序生成的GUID"),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    UserIdentity = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Data = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: true),
                    IdentityType = table.Column<int>(type: "INTEGER", nullable: false),
                    MobilePhone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true, defaultValue: ""),
                    MobileAreaCode = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true, defaultValue: ""),
                    MobileConfirmed = table.Column<bool>(type: "INTEGER", nullable: true),
                    MobileConfirmedTimeUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true, defaultValue: ""),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: true),
                    EmailConfirmedTimeUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_user_identity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sys_user_realname",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "主键，最好是顺序生成的GUID"),
                    IdCardIdentity = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Data = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: true),
                    IdCardType = table.Column<int>(type: "INTEGER", nullable: false),
                    IdCard = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    IdCardRealName = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    IdCardFrontUrl = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    IdCardBackUrl = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    IdCardHandUrl = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    StartTimeUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EndTimeUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IdCardStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    IdCardConfirmed = table.Column<bool>(type: "INTEGER", nullable: true),
                    IdCardConfirmedTimeUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ConfirmerId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_user_realname", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sys_validation_token",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "主键，最好是顺序生成的GUID"),
                    Token = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_validation_token", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sys_wx_union",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "主键，最好是顺序生成的GUID"),
                    Platform = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    AppId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    OpenId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    UnionId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_wx_union", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sys_activity_log_CreationTime",
                table: "sys_activity_log",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_sys_admin_CreationTime",
                table: "sys_admin",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_sys_admin_role_CreationTime",
                table: "sys_admin_role",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_sys_app_AppKey",
                table: "sys_app",
                column: "AppKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sys_app_CreationTime",
                table: "sys_app",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_sys_app_scope_CreationTime",
                table: "sys_app_scope",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_sys_department_CreationTime",
                table: "sys_department",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_sys_department_IsDeleted",
                table: "sys_department",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_sys_department_LastModificationTime",
                table: "sys_department",
                column: "LastModificationTime");

            migrationBuilder.CreateIndex(
                name: "IX_sys_department_ParentId",
                table: "sys_department",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_sys_department_assignment_CreationTime",
                table: "sys_department_assignment",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_sys_external_access_token_CreationTime",
                table: "sys_external_access_token",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_sys_external_connect_CreationTime",
                table: "sys_external_connect",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_sys_job_CreationTime",
                table: "sys_job",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_sys_menu_CreationTime",
                table: "sys_menu",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_sys_menu_LastModificationTime",
                table: "sys_menu",
                column: "LastModificationTime");

            migrationBuilder.CreateIndex(
                name: "IX_sys_menu_ParentId",
                table: "sys_menu",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_sys_notification_CreationTime",
                table: "sys_notification",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_sys_permission_CreationTime",
                table: "sys_permission",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_sys_region_CreationTime",
                table: "sys_region",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_sys_region_ParentId",
                table: "sys_region",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_sys_resource_acl_CreationTime",
                table: "sys_resource_acl",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_sys_role_CreationTime",
                table: "sys_role",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_sys_role_permission_CreationTime",
                table: "sys_role_permission",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_sys_sequence_Category",
                table: "sys_sequence",
                column: "Category",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sys_sequence_CreationTime",
                table: "sys_sequence",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_sys_sequence_LastModificationTime",
                table: "sys_sequence",
                column: "LastModificationTime");

            migrationBuilder.CreateIndex(
                name: "IX_sys_settings_CreationTime",
                table: "sys_settings",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_sys_storage_meta_CreationTime",
                table: "sys_storage_meta",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_sys_storage_meta_IsDeleted",
                table: "sys_storage_meta",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_sys_storage_meta_LastModificationTime",
                table: "sys_storage_meta",
                column: "LastModificationTime");

            migrationBuilder.CreateIndex(
                name: "IX_sys_user_CreationTime",
                table: "sys_user",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_sys_user_IdentityName",
                table: "sys_user",
                column: "IdentityName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sys_user_IsDeleted",
                table: "sys_user",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_sys_user_LastModificationTime",
                table: "sys_user",
                column: "LastModificationTime");

            migrationBuilder.CreateIndex(
                name: "IX_sys_user_address_CreationTime",
                table: "sys_user_address",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_sys_user_address_IsDeleted",
                table: "sys_user_address",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_sys_user_identity_CreationTime",
                table: "sys_user_identity",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_sys_user_identity_UserId",
                table: "sys_user_identity",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_sys_user_identity_UserIdentity",
                table: "sys_user_identity",
                column: "UserIdentity",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sys_user_realname_CreationTime",
                table: "sys_user_realname",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_sys_user_realname_IdCardIdentity",
                table: "sys_user_realname",
                column: "IdCardIdentity",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sys_user_realname_UserId",
                table: "sys_user_realname",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_sys_validation_token_CreationTime",
                table: "sys_validation_token",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_sys_validation_token_Token",
                table: "sys_validation_token",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sys_wx_union_CreationTime",
                table: "sys_wx_union",
                column: "CreationTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sys_activity_log");

            migrationBuilder.DropTable(
                name: "sys_admin");

            migrationBuilder.DropTable(
                name: "sys_admin_role");

            migrationBuilder.DropTable(
                name: "sys_app");

            migrationBuilder.DropTable(
                name: "sys_app_scope");

            migrationBuilder.DropTable(
                name: "sys_department");

            migrationBuilder.DropTable(
                name: "sys_department_assignment");

            migrationBuilder.DropTable(
                name: "sys_external_access_token");

            migrationBuilder.DropTable(
                name: "sys_external_connect");

            migrationBuilder.DropTable(
                name: "sys_job");

            migrationBuilder.DropTable(
                name: "sys_menu");

            migrationBuilder.DropTable(
                name: "sys_notification");

            migrationBuilder.DropTable(
                name: "sys_permission");

            migrationBuilder.DropTable(
                name: "sys_region");

            migrationBuilder.DropTable(
                name: "sys_resource_acl");

            migrationBuilder.DropTable(
                name: "sys_role");

            migrationBuilder.DropTable(
                name: "sys_role_permission");

            migrationBuilder.DropTable(
                name: "sys_sequence");

            migrationBuilder.DropTable(
                name: "sys_settings");

            migrationBuilder.DropTable(
                name: "sys_storage_meta");

            migrationBuilder.DropTable(
                name: "sys_user");

            migrationBuilder.DropTable(
                name: "sys_user_address");

            migrationBuilder.DropTable(
                name: "sys_user_identity");

            migrationBuilder.DropTable(
                name: "sys_user_realname");

            migrationBuilder.DropTable(
                name: "sys_validation_token");

            migrationBuilder.DropTable(
                name: "sys_wx_union");
        }
    }
}
