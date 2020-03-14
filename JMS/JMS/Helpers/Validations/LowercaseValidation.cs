using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JMS.Helpers.Validations
{
    public class LowercaseAttribute : RegularExpressionAttribute
    {
        public LowercaseAttribute():base(@"^(?=.*[a-z]).{1,}$")
        {

        }
    }
    public class UppercaseAttribute : RegularExpressionAttribute
    {
        public UppercaseAttribute() : base(@"^(?=.*[A-Z]).{1,}$")
        {

        }
    }
    public class SpecialcharAttribute : RegularExpressionAttribute
    {
        public SpecialcharAttribute() : base(@"^(?=.*[\d]).{1,}$")
        {

        }
    }
}
