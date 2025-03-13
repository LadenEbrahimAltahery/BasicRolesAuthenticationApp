﻿using System.ComponentModel.DataAnnotations;

namespace RolesBasedAuthentication.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }
        [Required,StringLength(50)]
        public string Name { get; set; }
        public string Description { get; set; } 


        public ICollection<UserRole>UserRoles { get; set; }=new List<UserRole>();
    }
}
