using AnyoneForTennis.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnyoneForTennis.Models;

public class Member
{
    [Key]
    public int MemberId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    [Required] 
    public string Email { get; set; } = string.Empty;

    public bool Active { get; set; } = true;
    public string? UserId { get; set; }

    [ForeignKey("UserId")]
    [ValidateNever]
    public virtual ApplicationUser? User { get; set; }
}
