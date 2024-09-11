using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POEPart3
{
    public class ModuleStudyRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RecordId { get; set; }
        [DisplayName("Module Code")]
        public string ModuleCode { get; set; }
        public DateTime Date { get; set; }
        [DisplayName("Hours Worked")]
        public int HoursWorked { get; set; }
        [DisplayName("Hours Left")]
        public int HoursLeft { get; set; }
        public string UserId { get; set; }

        //Foreign Keys
        [ForeignKey(nameof(Module))]
        public int ModuleId { get; set; }
        public Module Module { get; set; }

    }
}
