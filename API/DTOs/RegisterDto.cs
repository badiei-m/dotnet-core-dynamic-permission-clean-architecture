﻿using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class RegisterDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [RegularExpression("(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{4,8}$",ErrorMessage ="Password Must Be Complex.")]
    public string Password { get; set; }
    [Required]
    public string DisplayName { get; set; }
    [Required]
    public string UserName { get; set; }
}