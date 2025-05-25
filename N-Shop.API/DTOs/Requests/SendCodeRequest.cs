﻿using System.ComponentModel.DataAnnotations;

namespace N_Shop.API.DTOs.Requests;

public class SendCodeRequest
{
    public string Email { get; set; }
    public string Code { get; set; }
    public string Password { get; set; }
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; }
}