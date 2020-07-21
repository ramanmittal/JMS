using JMS.Service.SMS;
using System;
using System.Collections.Generic;
using System.Text;

namespace JMS.Service.ServiceContracts
{
    public interface ISMSService
    {
        bool Send(SMSDetails details);
    }
}
