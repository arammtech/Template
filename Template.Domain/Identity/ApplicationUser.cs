﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Template.Domain.Identity
{
	public class ApplicationUser : IdentityUser<int>
	{
        [MaxLength(50)]
        public string FirstName { get; set; } = null!;

        [MaxLength(50)]
        public string LastName { get; set; } = null!;

        public string? ImagePath { get; set; }
    }
}
