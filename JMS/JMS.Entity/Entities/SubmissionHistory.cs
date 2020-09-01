using System;
using System.Collections.Generic;
using System.Text;

namespace JMS.Entity.Entities
{
    public class SubmissionHistory
    {
        public long Id { get; set; }
        public long SubmissionId { get; set; }
        public string Action { get; set; }
        public long TenanatID { get; set; }
        public string ActorName { get; set; }
        public string ActorEmail { get; set; }
        public DateTime ActionDate { get; set; }
        public virtual Tenant Tenant { get; set; }
    }
}
