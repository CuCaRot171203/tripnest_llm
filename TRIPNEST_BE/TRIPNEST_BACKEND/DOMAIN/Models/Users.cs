using System;
using System.Collections.Generic;

namespace DOMAIN.Models;

public partial class Users
{
    public Guid UserId { get; set; }

    public int? RoleId { get; set; }

    public string Email { get; set; } = null!;

    public string? PasswordHash { get; set; }

    public string? FullName { get; set; }

    public string? Phone { get; set; }

    public string? Locale { get; set; }

    public string? ProfilePhotoUrl { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Auditlogs> Auditlogs { get; set; } = new List<Auditlogs>();

    public virtual ICollection<Bookings> Bookings { get; set; } = new List<Bookings>();

    public virtual ICollection<Companies> Companies { get; set; } = new List<Companies>();

    public virtual ICollection<Companyemployees> Companyemployees { get; set; } = new List<Companyemployees>();

    public virtual ICollection<Itineraries> Itineraries { get; set; } = new List<Itineraries>();

    public virtual ICollection<Messages> MessagesFromUser { get; set; } = new List<Messages>();

    public virtual ICollection<Messages> MessagesToUser { get; set; } = new List<Messages>();

    public virtual ICollection<Notifications> Notifications { get; set; } = new List<Notifications>();

    public virtual ICollection<Refreshtokens> Refreshtokens { get; set; } = new List<Refreshtokens>();

    public virtual ICollection<Reviews> Reviews { get; set; } = new List<Reviews>();

    public virtual Roles? Role { get; set; }
}
