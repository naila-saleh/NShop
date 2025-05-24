using N_Shop.API.Models;

namespace N_Shop.API.DTOs.Responses;

public class UserResponse
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public ApplicationUserGender Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
}