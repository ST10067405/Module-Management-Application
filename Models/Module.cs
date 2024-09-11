using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POEPart3
{
    public class Module
    {
        [Key]
        [Required]
        [DisplayName("Module Id")]
        public int ModuleId { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int Credits { get; set; }
        [Required]
        [DisplayName("Class Hours Per Week")]
        public int ClassHoursPerWeek { get; set; }
        [Required]
        [DisplayName("Self Study Hours Per Week")]
        public int SelfStudyHoursPerWeek { get; set; }
        public string UserId { get; set; }

        //Foreign Keys
        [ForeignKey(nameof(Semester))]
        [DisplayName("Semester")]
        public int SemesterId { get; set; }
        public Semester Semester { get; set; }

    }
}
