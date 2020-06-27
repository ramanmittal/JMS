using System;
using System.Collections.Generic;
using System.Text;

namespace JMS.ViewModels.Users
{
    public class UserGridModel
    {
        public int TotalItems { get; set; }
        public IEnumerable<UserGridRowModel> Data { get; set; }
    }
    public class UserGridRowModel
    {
        public UserGridRowModel()
        {
            Roles = new List<string>();
        }
        public long UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string ProfileImage { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public bool? IsDisabled { get; set; }
    }
}
