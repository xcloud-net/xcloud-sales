namespace XCloud.Core.Helper;

public class MyStringComparer : IComparer<string>
{
    public int Compare(string x, string y)
    {
        if (x == null && y == null) { return 0; }
        if (x != null && y == null) { return 1; }
        if (x == null && y != null) { return -1; }

        int i = 0;
        while (i < x.Length && i < y.Length)
        {
            if (x[i] > y[i])
            {
                return 1;
            }
            else if (x[i] < y[i])
            {
                return -1;
            }
            ++i;
        }

        if (x.Length > y.Length)
        {
            return 1;
        }
        else if (x.Length == y.Length)
        {
            return 0;
        }
        else
        {
            return -1;
        }
    }
}