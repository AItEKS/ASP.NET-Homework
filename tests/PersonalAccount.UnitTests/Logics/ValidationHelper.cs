using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PersonalAccount.UnitTests.Logics; 

public static class ValidationHelper
{
    public static IList<ValidationResult> Validate(object model)
    {
        var results = new List<ValidationResult>();
        var ctx = new ValidationContext(model);
        Validator.TryValidateObject(model, ctx, results, true);
        return results;
    }
}