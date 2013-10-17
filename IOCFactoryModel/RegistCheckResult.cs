using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IOCFactoryModel
{
    public class RegistCheckResult
    {
        public bool IsPass { get; set; }

        public string Message { get; set; }

        public RegistCheckResult()
        {
            this.IsPass = true;
        }

        public static implicit operator bool(RegistCheckResult obj)
        {
            return obj.IsPass;
        }

        public static implicit operator RegistCheckResult(bool result)
        {
            return new RegistCheckResult() { IsPass = result, Message = null };
        }
    }
}
