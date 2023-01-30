using NorthwindSample.Attribute;
using System.ComponentModel.DataAnnotations;

namespace NorthwindSample.Models;

[MetadataType(typeof(UserMetaData))]
public partial class User : BaseEntity
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string LastName { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public string PasswordHash { get; set; }

    public string Email { get; set; }

    public string Gsm { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; } = new List<UserRole>();
}