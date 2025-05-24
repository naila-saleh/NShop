using System.ComponentModel.DataAnnotations;

namespace N_Shop.API.Validations;

public class OverYears(int year=18):ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is DateTime date)
        {
            if (DateTime.Now.Year - date.Year >= year) return true;
        }

        return false;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} must be at least {year} years old.";
    }
}