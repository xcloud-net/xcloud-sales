using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XCloud.Sales.Extension;
using XCloud.Sales.Service.Catalog;

namespace XCloud.Test.Mall;

[TestClass]
public class GoodsSpecTest
{
    [TestMethod]
    public void GenerateCombination()
    {
        var mappings = new[] {
            new SpecCombinationItemDto() { SpecId=1,SpecValueId=1 },
            new SpecCombinationItemDto() { SpecId=1,SpecValueId=2 },
            new SpecCombinationItemDto() { SpecId=1,SpecValueId=3 },
            new SpecCombinationItemDto() { SpecId=1,SpecValueId=4 },
            new SpecCombinationItemDto() { SpecId=1,SpecValueId=5 },

            new SpecCombinationItemDto() { SpecId=2,SpecValueId=6 },
            new SpecCombinationItemDto() { SpecId=2,SpecValueId=7 },

            new SpecCombinationItemDto() { SpecId=3,SpecValueId=8 },

            new SpecCombinationItemDto() { SpecId=4,SpecValueId=9 },
            new SpecCombinationItemDto() { SpecId=4,SpecValueId=10 },
        };

        string GetString(SpecCombinationItemDto[] data) =>
            string.Join('\n', data.Select(x => $"{x.SpecId}={x.SpecValueId}"));

        var combines = mappings.GenerateCombination().Select(GetString).ToArray();

    }
}