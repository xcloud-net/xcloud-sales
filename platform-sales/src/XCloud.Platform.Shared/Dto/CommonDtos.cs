using Volo.Abp.Application.Dtos;

namespace XCloud.Platform.Shared.Dto;

public class CreateNoByCategoryDto : IEntityDto
{
    public CreateNoByCategoryDto() { }

    public CreateNoByCategoryDto(string category)
    {
        this.Category = category;
    }

    public string Category { get; set; }
    public string Description { get; set; }
    public string Modifier { get; set; }
    public int? MinId { get; set; }

    public static implicit operator CreateNoByCategoryDto(string category) =>
        new CreateNoByCategoryDto(category);
}