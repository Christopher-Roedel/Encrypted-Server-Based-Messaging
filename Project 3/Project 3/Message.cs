using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_3
{
    /// <summary>
    /// An object to hold messages when needed by the program.
    /// Created by deserializing message recieved from server.
    /// NOTE: I know that the writeup doesn't include date time
    /// and it doesn't get used by the program, but the server
    /// sends a date time so I added it here.
    /// </summary>
    internal class Message
    {
        public string email { get; set; }
        public string content { get; set; }
        public DateTime time { get; set; }
    }
}
