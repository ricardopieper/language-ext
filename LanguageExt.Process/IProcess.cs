﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    /// <summary>
    /// Represents a process as an object rather than a function
    /// </summary>
    /// <typeparam name="T">Message type</typeparam>
    public interface IProcess<in T>
    {
        /// <summary>
        /// Inbox message handler
        /// </summary>
        /// <param name="msg">Message</param>
        void OnMessage(T msg);
    }
}
