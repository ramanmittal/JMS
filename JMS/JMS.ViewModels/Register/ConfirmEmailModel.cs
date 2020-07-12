using System;
using System.Collections.Generic;
using System.Text;

namespace JMS.ViewModels.Register
{
    public class ConfirmEmailModel
    {
        public long UserId { get; set; }
        public string Token { get; set; }
    }
}
