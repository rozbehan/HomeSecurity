using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeSecurityApi
{
    public class SecureParameters
    {
        public SecureParameters()
        {

        }
        public bool Passed { get; private set; }

        public SecureParameters ValidateCode(string code)
        {
            Passed = true;
            return this;
        }
        public void ChangeCurrentState(string code)
        {

        }
    }

}
