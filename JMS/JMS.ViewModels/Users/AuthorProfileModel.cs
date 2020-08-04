using JMS.ViewModels.SystemAdmin;
using System;
using System.Collections.Generic;
using System.Text;

namespace JMS.ViewModels.Users
{
    public class AuthorProfileModel: SystemAdminProfileModel
    {
        
        public string ORCID { get; set; }
    }
}
