using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace AnyoneForTennis.Models;

public class Schedule
{
    [Key]
    public int ScheduleId { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Location { get; set; }

    public string? Description { get; set; }

    public DateTime ScheduledOn { get; set; }

    public int CoachId { get; set; }
    [ValidateNever]
    public virtual Coach Coach { get; set; }
}
