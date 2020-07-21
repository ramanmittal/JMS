using System;
using System.Collections.Generic;
using System.Text;

namespace JMS.Service.SMS
{
    public class SMSDetails
    {
        public List<string> PhoneNumber { get; set; }
        public string Message { get; set; }
    }
}
