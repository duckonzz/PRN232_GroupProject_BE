using GenderHealthCare.Core.Models;

namespace GenderHealthCare.Entity
{
    public class QAThread : BaseEntity
    {
        public string Question { get; set; }
        public string? Answer { get; set; }
        public DateTimeOffset? AnsweredAt { get; set; }
        public string CustomerId { get; set; }
        /*public string ConsultantId { get; set; }*/

        public User Customer { get; set; }
        /*public Consultant Consultant { get; set; }*/
    }
}
