# 账号体系(基础组件)

1. admin 公司（管理加盟店，仓库，库存，商品）
2. vendor 加盟店（管理店铺和员工信息）
3. employee 加盟店员工（登陆员工账号工作）
4. user 顾客-会员账号（使用会员消费）

# todo
剥离数据访问层

# tips

单租户下，写一个定时任务ensure默认租户是否存在，并关闭其他租户

# aliyun sms

```xml
		<PackageReference Include="aliyun-net-sdk-core" Version="1.5.10" />
```

```csharp
//发送短信
                IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", get_config("key"), get_config("secret"));
                DefaultAcsClient client = new DefaultAcsClient(profile);
                CommonRequest request = new CommonRequest();
                request.Method = MethodType.POST;
                request.Domain = "dysmsapi.aliyuncs.com";
                request.Version = "2017-05-25";
                request.Action = "SendSms";
                // request.Protocol = ProtocolType.HTTP;
                request.AddQueryParameters("PhoneNumbers", context.Message.Phone);
                request.AddQueryParameters("SignName", get_config("sign_name"));
                request.AddQueryParameters("TemplateCode", get_config("template_code"));
                request.AddQueryParameters("TemplateParam", this.context.JsonSerializer.SerializeToString(new { code = context.Message.Code }));

                var response = client.GetCommonResponse(request);
                var sms_res_content = System.Text.Encoding.Default.GetString(response.HttpResponse.Content);
                sms_res_content.Should().NotBeNullOrEmpty();

                var sms_res = this.context.JsonSerializer.DeserializeFromString<AliyunSmsRes>(sms_res_content);
                if (sms_res.Code?.ToLower() != "ok")
                {
                    throw new UserFriendlyException(sms_res.Message ?? "sms error");
                }
```