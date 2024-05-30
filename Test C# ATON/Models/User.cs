﻿using System;

namespace UserService.Models
{
    public class User
    {
        public Guid Guid { get; set; } = Guid.NewGuid();
        public string Login { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public int Gender { get; set; } // 0 - female, 1 - male, 2 - no info
        public DateTime? Birthday { get; set; }
        public bool Admin {  get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public string CreatedBy {  get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? RevokedOn { get; set; }
        public string RevokedBy { get; set; }

    }
}
