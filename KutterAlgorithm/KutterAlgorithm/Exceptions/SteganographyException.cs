using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steganography.Exceptions
{
    public class SteganographyException : Exception
    {
        public SteganographyException() : base()
        {
            
        }

        public SteganographyException(string msg) : base(msg)
        {
            
        }
    }
}
