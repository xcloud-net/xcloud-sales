using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace XCloud.AspNetMvc.ModelValidation;

public class MyModelValidatorProvider : IModelValidatorProvider
{
    public void CreateValidators(ModelValidatorProviderContext context)
    {
        context.Results.Add(new ValidatorItem()
        {
            IsReusable = false,
            Validator = new MyModelValidator()
        });
    }
}