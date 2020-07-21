using JMS.Service.ServiceContracts;
using JMS.Service.SMS;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Twilio.Clients;
using Twilio;
using Twilio.Types;
using Twilio.Rest.Api.V2010.Account;
using Microsoft.Extensions.Logging;
using JMS.Service.Settings;

namespace JMS.Service.Services
{
    public class TwilioSmsService : ISMSService
    {
        private readonly IServiceProvider _serviceProvider;
        public TwilioSmsService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public bool Send(SMSDetails details)
        {
            return SendMessage(details.Message,details.PhoneNumber);
        }
        private bool SendMessage(string message, List<string> phoneNumbers)
        {
            var configuration = _serviceProvider.GetService<IConfiguration>();
            bool returnValue = true;
            var countryCode = configuration[JMSSetting.TwilioCountryCode.ToString()];
            var accountSid = configuration[JMSSetting.TwilioAccountSid.ToString()];
            var authToken = configuration[JMSSetting.TwilioAuthToken.ToString()];
            var twilioPhoneNumber = configuration[JMSSetting.TwilioPhoneNumber.ToString()];
            var logger = _serviceProvider.GetService<ILogger<TwilioSmsService>>();
            TwilioClient.Init(accountSid, authToken);
            foreach (var phoneNumber in phoneNumbers)
            {
                var countryPhoneNumber = countryCode + phoneNumber;
                try
                {
                    var message1 = MessageResource.Create(to: new PhoneNumber(countryPhoneNumber), from: new PhoneNumber(twilioPhoneNumber), body: message);
                }
                catch (Exception ex)
                {
                    returnValue = false;
                    logger.LogError(ex, ex.Message);
                }
            }


            return returnValue;
        }
    }
}
