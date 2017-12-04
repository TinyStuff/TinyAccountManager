using System;
using System.Collections.Generic;
using System.Text;

namespace TinyAccountManager.Abstraction
{
    public class Account
    {
        public string ServiceId { get; set; }
        public string Username { get; set; }
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
    }
}
