using System;

namespace Data.Abstractions.Exceptions
{
    /// <summary>
    /// Exception appears when data file is not found on server
    /// </summary>
    public class ServerDataFileNotFoundException : Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fileName">Name of file which is not found</param>
        /// <param name="message">Message of exception</param>
        public ServerDataFileNotFoundException(string fileName, string message) : base(message) 
        {
            FileName = fileName;
        }

        /// <summary>
        /// Name of file which is not found
        /// </summary>
        public string FileName { get; private set; }
    }
}
