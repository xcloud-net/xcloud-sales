using System.Text;
using Volo.Abp.Validation;

namespace XCloud.Core.ExceptionHandler;

public interface IExceptionDetailContributor
{
    void Contibute(Exception e, ref StringBuilder sb);
}

public abstract class ExceptionContributorBase<E> : IExceptionDetailContributor where E : Exception
{
    public void Contibute(Exception e, ref StringBuilder sb)
    {
        if (e is E err && err != null)
        {
            this.ContibuteDetail(err, ref sb);
        }
    }

    public abstract void ContibuteDetail(E e, ref StringBuilder sb);
}

public class AbpValidationExceptionContributor : ExceptionContributorBase<AbpValidationException>
{
    public override void ContibuteDetail(AbpValidationException e, ref StringBuilder sb)
    {
        var errors = e.ValidationErrors.Select(x => $"{x.MemberNames}->{x.ErrorMessage}").ToArray();
        foreach (var m in errors)
        {
            sb.AppendLine(m);
        }
    }
}