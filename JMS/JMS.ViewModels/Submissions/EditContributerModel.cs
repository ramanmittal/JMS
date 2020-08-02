using JMS.Entity.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace JMS.ViewModels.Submissions
{
    public class EditContributerModel
    {
        public long ContributerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public string ORCIDiD { get; set; }
        public string AffiliationNo { get; set; }
        public ContributerRole ContributerRole { get; set; }
    }
}
