using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JMS.ViewModels.Users
{
    public class UserGridSearchModel
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value == null ? "" : value.Trim().ToLower(); }
        }

        private string email;

        public string Email
        {
            get { return email; }
            set { email = value == null ? "" : value.Trim().ToLower(); }
        }

        public string Roles { get; set; }
        
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        public bool? Status { get; set; }
        public string sortField { get; set; }
        public string sortOrder { get; set; }
    }
}
