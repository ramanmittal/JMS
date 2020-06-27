using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Text;

namespace JMS.Service.Enums
{
    public enum Role
    {
        SystemAdmin,
        Admin,
        EIC,
        SectionEditor,
        Reviewer
    }
    public static class RoleHelper
    {
        public static IDictionary<int,string> GetRolesForUser()
        {
           return Enum.GetValues(typeof(Role)).Cast<Role>().ToDictionary(x => (int)x, x => x.ToString());
        }
    }
}
