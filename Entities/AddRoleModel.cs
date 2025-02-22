﻿using System.ComponentModel.DataAnnotations;

namespace MoviesApi.Entities
{
    public class AddRoleModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string RoleName { get; set; }
    }
}