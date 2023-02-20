using FluentAssertions;
using XCloud.Core.Dto;

namespace XCloud.Core.Extension;

public static class TreeExtension
{
    public static void WalkTreeNodes<T>(this IEnumerable<T> flatNodes,
        Func<T, string> idSelector,
        Func<T, string> parentIdSelector,
        Action<T, int, T[]> nodeAndLevelAndChildrenCallback,
        string firstNodeId = null)
    {
        var repeatCheck = new List<string>();

        void WalkNode(T node, int currentLevel)
        {
            var id = idSelector.Invoke(node);

            repeatCheck.AddOnceOrThrow(id, error_msg: "树存在无限递归");

            var children = flatNodes.Where(x => parentIdSelector.Invoke(x) == id).ToArray();

            nodeAndLevelAndChildrenCallback.Invoke(node, currentLevel, children);

            foreach (var child in children)
                WalkNode(child, currentLevel + 1);
        }

        if (string.IsNullOrWhiteSpace(firstNodeId))
        {
            //walk from roots
            var roots = flatNodes.Where(x => string.IsNullOrWhiteSpace(parentIdSelector.Invoke(x))).ToArray();

            foreach (var root in roots)
                WalkNode(root, 1);
        }
        else
        {
            var firstNode = flatNodes.FirstOrDefault(x => idSelector.Invoke(x) == firstNodeId);
            if (firstNode == null)
                throw new ArgumentException("first node id dose not exist in tree");

            var parentNodes = flatNodes.FindNodeParents(idSelector, parentIdSelector, firstNodeId);
            var currentLevel = parentNodes.Length;

            WalkNode(firstNode, currentLevel);
        }
    }

    public static T[] FindNodeParents<T>(this IEnumerable<T> list,
        Func<T, string> idSelector,
        Func<T, string> parentIdSelector,
        string nodeId)
    {
        var parents = new List<T>();
        var repeatCheck = new List<string>();

        var current_uid = nodeId;

        while (true)
        {
            repeatCheck.AddOnceOrThrow(current_uid);

            var current_node = list.FirstOrDefault(x => idSelector.Invoke(x) == current_uid);
            if (current_node == null)
                break;

            parents.Insert(0, current_node);

            //next
            current_uid = parentIdSelector.Invoke(current_node);
        }

        return parents.ToArray();
    }


    #region 非递归实现
    /// <summary>
    /// 深度优先
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static IEnumerable<KeyValuePair<T, T[]>> FindNodeChildrenRecursivelyDeepFirst<T>(this IEnumerable<T> data_source,
        Func<T, string> idSelector,
        Func<T, string> parentIdSelector,
        string firstNodeId)
    {
        var repeatCheck = new List<string>();
        var firstNode = data_source.FirstOrDefault(x => idSelector.Invoke(x) == firstNodeId);
        firstNode.Should().NotBeNull(nameof(firstNodeId));

        var stack = new Stack<T>();
        stack.Push(firstNode);

        while (true)
        {
            if (!stack.TryPop(out var data))
                break;

            var id = idSelector.Invoke(data);

            repeatCheck.AddOnceOrThrow(id);

            var children = data_source.Where(x => parentIdSelector.Invoke(x) == id).ToArray();

            foreach (var child in children)
                stack.Push(child);

            yield return new KeyValuePair<T, T[]>(data, children);
        }
    }

    /// <summary>
    /// 广度优先
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static IEnumerable<KeyValuePair<T, T[]>> FindNodeChildrenRecursivelyWideFirst<T>(this IEnumerable<T> data_source,
        Func<T, string> idSelector,
        Func<T, string> parentIdSelector,
        string firstNodeId)
    {
        var repeatCheck = new List<string>();
        var firstNode = data_source.FirstOrDefault(x => idSelector.Invoke(x) == firstNodeId);
        firstNode.Should().NotBeNull(nameof(firstNodeId));

        var queue = new Queue<T>();
        queue.Enqueue(firstNode);

        while (true)
        {
            if (!queue.TryDequeue(out var data))
                break;

            var id = idSelector.Invoke(data);

            repeatCheck.AddOnceOrThrow(id);

            var children = data_source.Where(x => parentIdSelector.Invoke(x) == id).ToArray();

            foreach (var child in children)
                queue.Enqueue(child);

            yield return new KeyValuePair<T, T[]>(data, children);
        }
    }
    #endregion

    public static IEnumerable<AntDesignTreeNode> BuildAntTree<T>(this IEnumerable<T> list,
        Func<T, string> idSelector,
        Func<T, string> parentIdSelector,
        Func<T, string> titleSelector,
        Func<T, object> dataSelector = null)
    {
        dataSelector ??= (x => x);

        var handledNodes = new List<string>();

        AntDesignTreeNode BindChildren(T node, int level)
        {
            var nodeId = idSelector.Invoke(node);
            nodeId.Should().NotBeNullOrEmpty("每个节点都需要id");
            handledNodes.AddOnceOrThrow(nodeId, "树存在错误");

            var children = list.Where(x => parentIdSelector.Invoke(x) == nodeId).ToArray();

            return new AntDesignTreeNode()
            {
                key = nodeId,
                title = titleSelector.Invoke(node),
                raw_data = dataSelector.Invoke(node),
                node_level = level,
                //递归下一级
                children = children.Select(x => BindChildren(x, level + 1)).ToArray()
            };
        }

        var data = list.Where(x => string.IsNullOrWhiteSpace(parentIdSelector.Invoke(x))).ToArray();

        var res = data.Select(x => BindChildren(x, 1)).ToArray();

        return res;
    }

    public static IEnumerable<KeyValuePair<string, T>> BuildFlatTitleList<T>(this IEnumerable<T> list,
        Func<T, string> idSelector,
        Func<T, string> parentIdSelector,
        Func<T, string> titleSelector,
        string split = null)
    {
        if (string.IsNullOrWhiteSpace(split))
        {
            split = ">>";
        }
        var flatList = new List<KeyValuePair<string, T>>();
        var handledNodes = new List<string>();

        void BindChildren(string parentTitle, T node, int level)
        {
            var nodeId = idSelector.Invoke(node);
            nodeId.Should().NotBeNullOrEmpty("每个节点都需要id");
            handledNodes.AddOnceOrThrow(nodeId, "树存在错误");

            var title = titleSelector.Invoke(node);
            title = string.Join(split, new string[] { parentTitle, title }.WhereNotEmpty().ToArray());

            flatList.Add(new KeyValuePair<string, T>(title, node));

            var children = list.Where(x => parentIdSelector.Invoke(x) == nodeId).ToArray();

            foreach (var m in children)
            {
                BindChildren(title, m, level + 1);
            }
        }

        var data = list.Where(x => string.IsNullOrWhiteSpace(parentIdSelector.Invoke(x))).ToList();

        data.ForEach(x => BindChildren(null, x, 1));

        flatList = flatList.OrderBy(x => x.Key).ToList();

        return flatList;
    }
}