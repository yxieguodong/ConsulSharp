﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ConsulSharp
{
    /// <summary>
    /// ttl check
    /// </summary>
    public class TTLCheck : Check
    {
        /// <summary>
        /// ttl
        /// </summary>
        public string Ttl
        { get; set; }
        /// <summary>
        /// notes
        /// </summary>
        public string Notes
        { get; set; }
    }
}