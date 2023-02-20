using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace XCloud.AspNetMvc.ModelValidation;

public class MyModelValidator : IModelValidator
{
    public IEnumerable<ModelValidationResult> Validate(ModelValidationContext context)
    {
        return Array.Empty<ModelValidationResult>();
    }
}