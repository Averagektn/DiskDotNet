﻿using Disk.Service.Exceptions.Common;

namespace Disk.Service.Exceptions
{
    public class InvalidNameException : ServiceException 
    { 
        public InvalidNameException(string message) : base(message) { } 
        public InvalidNameException(string output, string message) : base(output, message) { }
    }
}