using XCloud.Core;

namespace XCloud.Database.EntityFrameworkCore;

public class UnSubmitChangesException : BaseException
{
    public UnSubmitChangesException(string msg) : base(msg) { }
}