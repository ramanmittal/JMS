using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace JMS.ViewModels.Admin
{
    public class AppearanceSettingsModel
    {
        [DisplayName("AboutUs Content")]

        public string AboutUsContent { get; set; }
        [DisplayName("Footer Content")]
        public string FooterContent { get; set; }
        [DisplayName("Additional Content")]
        public string AdditionalContent { get; set; }
        [DisplayName("Privacy Policy Content")]
        public string PrivacyPolicyContent { get; set; }
    }
}
