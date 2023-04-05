using XCloud.Core.Dto;

namespace XCloud.Core.Application.Entity;

public static class TreeEntityExtension
{
    public static IEnumerable<AntDesignTreeNode> BuildAntTree<T>(this IEnumerable<T> list, Func<T, string> title_selector, Func<T, object> raw_data = null) where T : TreeEntityBase
    {
        var res = list.BuildAntTree(
            x => x.Id,
            x => x.ParentId,
            title_selector,
            raw_data).ToArray();

        return res;
    }

    public static List<T> FindNodeChildrenRecursively<T>(this IEnumerable<T> dataSource, T firstNode)
        where T : TreeEntityBase
    {
        var list = new List<T>();
        dataSource.WalkTreeNodes(x => x.Id, x => x.ParentId, (node, level, children) => list.Add(node), firstNode.Id);
        return list;
    }

    /// <summary>
    /// 判断是父级节点
    /// </summary>
    /// <returns></returns>
    public static bool IsFirstLevel<T>(this T model) where T : TreeEntityBase
    {
        return string.IsNullOrWhiteSpace(model.ParentId?.Trim());
    }

    /// <summary>
    /// 修改节点层级和父级为第一级
    /// </summary>
    public static T AsFirstLevel<T>(this T model) where T : TreeEntityBase
    {
        model.ParentId = string.Empty;
        return model;
    }
}