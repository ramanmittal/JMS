using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JMS.Models
{
    public class UserMenu
    {
        public List<Menu> LoggedUserMenus { get; set; }
        public List<Menu> LoggedOutMenus { get; set; }
    }
    public class Menu
    {
        public Menu()
        {
            Roles = new List<string>();
            SubMenus = new List<SubMenu>();
        }
        public string Text { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string URL { get; set; }
        public List<string> Roles { get; set; }
        public List<SubMenu> SubMenus { get; set; }
    }
    public class SubMenu
    {
        public SubMenu()
        {
            Roles = new List<string>();
        }
        public string Text { get; set; }
        public List<string> Roles { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string URL { get; set; }
    }
}
