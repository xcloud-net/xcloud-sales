# 服务注册发现

```xml
	<ItemGroup>
		<PackageReference Include="Steeltoe.Discovery.ClientCore" Version="3.0.1" />
		<PackageReference Include="Steeltoe.Discovery.Consul" Version="3.0.1" />
	</ItemGroup>
```

```csharp
builder.Services.AddDiscoveryClient();
builder.Services.AddServiceDiscovery(option => option.UseConsul());
```