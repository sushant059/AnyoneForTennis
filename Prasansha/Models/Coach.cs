using AnyoneForTennis.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnyoneForTennis.Models;

public class Coach
{
    [Key]
    public int CoachId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public string? Biography { get; set; }

    public byte[]? Photo { get; set; }

    public string? UserId { get; set; }

    [ForeignKey("UserId")]
    [ValidateNever]
    public virtual ApplicationUser? User { get; set; }
}
