using System;
using System.Collections.Generic;
using System.Text;

namespace JMS.Entity.Entities
{
    public class Trackable
    {
        public DateTime? Added { get; set; }
        public DateTime? Changed { get; set; }
        public DateTime? Deleted { get; set; }
    }
}
