using XCloud.Core.Helper;

namespace XCloud.Sales.Services.Catalog;

public interface ISpecCombinationParser : ISalesAppService
{
    string FingerPrint(SpecCombinationItemDto m);

    string FingerPrint(SpecCombinationItemDto[] arr);

    string SerializeSpecCombination(SpecCombinationItemDto[] model);

    SpecCombinationItemDto[] DeserializeSpecCombination(string json);

    bool AreSpecCombinationEqual(string attributes1, string attributes2);

    bool AreSpecCombinationEqual(SpecCombinationItemDto[] attributes1, SpecCombinationItemDto[] attributes2);
}

[ExposeServices(typeof(ISpecCombinationParser))]
public class SpecCombinationParser : SalesAppService, ISpecCombinationParser
{
    public SpecCombinationParser()
    {
        //
    }

    public string FingerPrint(SpecCombinationItemDto m) => $"{m.SpecId}={m.SpecValueId}";

    public string FingerPrint(SpecCombinationItemDto[] arr)
    {
        var items = arr
            .OrderBy(x => x.SpecId)
            .ThenBy(x => x.SpecValueId)
            .Select(FingerPrint)
            .ToArray();

        var fingerPrint = string.Join('-', items);
        return fingerPrint;
    }

    public string SerializeSpecCombination(SpecCombinationItemDto[] model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        var json = this.JsonDataSerializer.SerializeToString(model);
        var dataCopy = this.JsonDataSerializer.DeserializeFromString<SpecCombinationItemDto[]>(json);

        //remove useless fields
        foreach (var m in dataCopy)
        {
            m.Spec = default;
            m.SpecValue = default;
        }

        //remove duplicate items
        dataCopy = dataCopy.DistinctBy(this.FingerPrint).ToArray();

        json = JsonDataSerializer.SerializeToString(dataCopy);
        return json;
    }

    public SpecCombinationItemDto[] DeserializeSpecCombination(string json)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(json) &&
                json.TrimStart().StartsWith("[") &&
                json.TrimEnd().EndsWith("]"))
            {
                var model = JsonDataSerializer.DeserializeFromString<SpecCombinationItemDto[]>(json);
                if (model != null)
                    return model;
            }
        }
        catch (Exception e)
        {
            this.Logger.LogWarning(message: e.Message, exception: e);
        }

        return Array.Empty<SpecCombinationItemDto>();
    }

    public virtual bool AreSpecCombinationEqual(SpecCombinationItemDto[] attributes1,
        SpecCombinationItemDto[] attributes2)
    {
        if (attributes1 == null || attributes2 == null)
            throw new ArgumentNullException(nameof(AreSpecCombinationEqual));

        if (attributes1.Length != attributes2.Length)
            return false;

        if (ValidateHelper.IsEmptyCollection(attributes1) && ValidateHelper.IsEmptyCollection(attributes2))
            return true;

        return FingerPrint(attributes1) == FingerPrint(attributes2);
    }

    public virtual bool AreSpecCombinationEqual(string attributes1, string attributes2)
    {
        var print1 = DeserializeSpecCombination(attributes1);

        var print2 = DeserializeSpecCombination(attributes2);

        return AreSpecCombinationEqual(print1, print2);
    }
}