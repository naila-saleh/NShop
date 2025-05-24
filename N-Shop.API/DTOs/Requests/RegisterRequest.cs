using System.ComponentModel.DataAnnotations;
using N_Shop.API.Models;
using N_Shop.API.Validations;

namespace N_Shop.API.DTOs.Requests;

public class RegisterRequest
{
    [MinLength(3)]
    public string FirstName { get; set; }
    [MinLength(3)]
    public string LastName { get; set; }
    [MinLength(5)]
    public string UserName { get; set; }
    [EmailAddress]
    public string Email { get; set; }
    public string Password { get; set; }
    [Compare(nameof(Password))]//[Compare("Password")]
    public string ConfirmPassword { get; set; }
    public ApplicationUserGender Gender { get; set; }
    [OverYears(18)]
    public DateTime DateOfBirth { get; set; }
}