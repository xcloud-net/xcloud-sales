using Volo.Abp.Application.Dtos;
using XCloud.Core.Dto;

namespace XCloud.Application.ServiceDiscovery;

public class ServiceDiscoveryResponseDto : ApiResponse<string>, IEntityDto
{
    public ServiceDiscoveryResponseDto()
    {
        //
    }

    public ServiceDiscoveryResponseDto(string address) : this()
    {
        this.SetData(address);
    }

    public bool Empty => string.IsNullOrWhiteSpace(this.Address);

    public string Address => this.Data;
}