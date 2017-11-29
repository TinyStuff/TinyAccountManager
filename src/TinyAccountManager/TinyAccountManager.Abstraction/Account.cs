﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TinyAccountManager.Abstraction
{
    public class Account
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public Dictionary<string, object> Properties { get; set; }
    }
}
