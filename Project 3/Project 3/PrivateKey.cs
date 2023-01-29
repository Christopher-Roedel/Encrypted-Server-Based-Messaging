using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_3
{
    /// <summary>
    /// An object to hold private keys when needed by the program.
    /// Created by deserializing the private.key file.
    /// </summary>
    internal class PrivateKey
    {
        public List<string> email { get; set; }
        public string key { get; set; }

    }
}
