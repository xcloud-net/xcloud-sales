using XCloud.Sales.Services.Catalog;

namespace XCloud.Sales.Services;

public static class Extensions
{
    public static IEnumerable<SpecCombinationItemDto[]> GenerateCombination(this SpecCombinationItemDto[] mappings,
        int? maxCount = null)
    {
        if (maxCount != null && maxCount.Value <= 0)
            throw new ArgumentException($"{nameof(GenerateCombination)}:{nameof(maxCount)}");

        if (!mappings.Any())
            yield break;

        var groupedData = mappings.GroupBy(x => x.SpecId)
            .Select(x => new { x.Key, Items = x.ToArray() }).ToArray();

        var groupedItems = groupedData.Select(x => x.Items).ToArray();

        var finalList = new List<SpecCombinationItemDto[]>();
        var temp = new SpecCombinationItemDto[groupedItems.Length];

        void Generate(int index)
        {
            var isLastLoop = index == groupedItems.Length - 1;

            var cursor = groupedItems[index].GetEnumerator();
            while (cursor.MoveNext())
            {
                if (maxCount != null && finalList.Count >= maxCount.Value)
                    break;

                var item = (SpecCombinationItemDto)cursor.Current;
                temp[index] = item;

                if (isLastLoop)
                {
                    //last loop
                    var currentCombination = new List<SpecCombinationItemDto>();
                    currentCombination.AddRange(temp);
                    finalList.Add(currentCombination.ToArray());
                }
                else
                {
                    Generate(index + 1);
                }
            }
        }

        //trigger
        Generate(0);

        foreach (var m in finalList)
            yield return m;
    }

}