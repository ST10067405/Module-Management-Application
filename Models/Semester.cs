using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace POEPart3
{
    public class Semester
    {
        [Key]
        [Required]
        public int SemesterId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [DisplayName("Number Of Weeks")]
        public int NumberOfWeeks { get; set; }
        [Required]
        [DisplayName("Start Date")]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public string UserId { get; set; }

    }  
}
