﻿using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class LoginRequest
{
    [Required]
    public string UserName { get; set; }

    [Required]
    public string Password { get; set; }
}