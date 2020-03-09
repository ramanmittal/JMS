using JMS.Entity.Data;
using JMS.Entity.Entities;
using JMS.Service.ServiceContracts;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Text;

namespace JMS.Service.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }
        public IEnumerable<ApplicationUser> GetTenantUserByRole(long tenantid, string role)
        {
            var users = _context.Users.Where(x => x.TenantId == tenantid);
            if (!string.IsNullOrEmpty(role))
            {
                var roleid = _context.Roles.Single(x => x.Name == role).Id;
                var roledUser = _context.UserRoles.Where(x => x.RoleId == roleid);
                users = from userRole in roledUser
                        join
                        user in users
                        on userRole.UserId equals user.Id
                        select user;

            }
            return users.ToList();
        }

        public ApplicationUser GetUser(long userId)
        {
            return _context.Users.Single(x => x.Id == userId);
        }
        public bool ValidateEmail(string email, long tenantId, long? userid)
        {
            var users = _context.Users.Where(x => x.TenantId == tenantId && x.Email == email);
            if (userid.HasValue)
            {
                users = users.Where(x => x.Id != userid.Value);
            }
            return !users.Any();
        }
    }
}
