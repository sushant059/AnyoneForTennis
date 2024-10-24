using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnyoneForTennis.Models
{
    public class Enrollment
    {
        [Key]
        public int EnrollmentId { get; set; }
        public int MemberId {  get; set; }
        [ForeignKey("MemberId")]
        [ValidateNever]
        public virtual Member Member { get; set; }
        public int ScheduleId {  get; set; }
        [ForeignKey("ScheduleId")]
        [ValidateNever]
        public virtual Schedule Schedule { get; set; }
    }
}
