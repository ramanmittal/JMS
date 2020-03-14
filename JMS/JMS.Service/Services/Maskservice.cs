using JMS.Service.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace JMS.Service.Services
{
    public class Maskservice: IMaskService
    {
        public string RemovePhoneMasking(string phoneNumber)
        {
            return phoneNumber.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
        }
    }
}
