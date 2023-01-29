using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_3
{
    /// <summary>
    /// An object to store a public key when needed by the program.
    /// Created by deserializing the public.key file.
    /// </summary>
    internal class PublicKey
    {
        public string email { get; set; }
        public string key { get; set; }
    }
}
