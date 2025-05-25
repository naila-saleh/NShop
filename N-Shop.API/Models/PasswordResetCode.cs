﻿namespace N_Shop.API.Models;

public class PasswordResetCode
{
    public int Id { get; set; }
    public string ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
    public string Code { get; set; }
    public DateTime ExpireDate { get; set; }
}