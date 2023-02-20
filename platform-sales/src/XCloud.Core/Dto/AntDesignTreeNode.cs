using Volo.Abp.Application.Dtos;

namespace XCloud.Core.Dto;

public class AntDesignTreeNode : IEntityDto
{
    public string key { get; set; }

    public string title { get; set; }

    public object raw_data { get; set; }

    public int? node_level { get; set; }

    public IEnumerable<AntDesignTreeNode> children { get; set; }
}

public class TreeNode
{
    public string Name { get; set; }
    public IEnumerable<TreeNode> Children { get; set; }
}