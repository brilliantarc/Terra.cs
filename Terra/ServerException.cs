using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terra
{
    /// <summary>
    /// Thrown when the Terra REST server returns an error.
    /// </summary>
    public class ServerException : System.ApplicationException
    {
        /// <summary>
        /// The HTTP status code returned by the Terra server.  Terra uses the
        /// HTTP status codes to indicate the error state that generated the
        /// problem.  You can use these error codes to interpret the problem
        /// and respond appropriately.
        /// 
        /// It should be noted that the error messages returned from Terra are
        /// designed to be "human-friendly", and in many cases may simply be
        /// displayed directly to the end user.  For those cases where you need
        /// to take additional action, checking the Status can be helpful.
        /// </summary>
        public System.Net.HttpStatusCode Status { get; set; }

        /// <summary>
        /// When trying to create a meme like a category or option, and a duplicate
        /// slug is found already existing in Terra, a Conflict is raised and
        /// returned.  When this happens, the conflicting meme is returned with
        /// the error message, and will be included in the Duplicate property of
        /// the exception.
        /// </summary>
        public Node Duplicate { get; set; }

        public ServerException(string message, System.Net.HttpStatusCode status, Node duplicate = null)
            : base(message)
        {
            Status = status;
            Duplicate = duplicate;
        }

        public override string ToString()
        {
            return Status + ": " + Message;
        }
    }
}
