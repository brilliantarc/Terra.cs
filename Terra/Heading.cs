using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terra
{
    /// <summary>
    /// An operating company's business sales heading.  Businesses are sold into
    /// a heading, and those headings are then mapped to taxonomic categories
    /// for additional information (properties and options).  Headings tend to
    /// be broad strokes, sales or market driven, whereas categories follow a
    /// stricter physical, ontological definition of information.
    /// </summary>
    public class Heading : Meme
    {
        /// <summary>
        /// The localized name of the heading
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The operating company's identifier for the heading
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// A third-party external identifier for the heading; typically
        /// not used, and need not be unique
        /// </summary>
        public string External { get; set; }

        /// <summary>
        /// The language of the heading name
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// The three or four letter code for the operating company owning
        /// this heading
        /// </summary>
        public string Opco { get; set; }

        /// <summary>
        /// An internal tracking code used to ensure that updates by the client
        /// do not overwrite changes made on the server between the time the
        /// client version of this heading was retrieved and when it was 
        /// written
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Returns "Heading" for the definition of the heading meme.  This is
        /// used internally; you should not need to refer to it in your client 
        /// code.
        /// </summary>
        public string Definition
        {
            get { return "Heading"; }
        }

        /// <summary>
        /// Returns the PID of the heading.  Used by the Meme interface to 
        /// generalize some server requests.  In your client code, you should
        /// try to refer to the heading's Pid, instead of its Slug.
        /// </summary>
        public string Slug
        {
            get { return Pid; }
        }

        public override int GetHashCode()
        {
            return ("heading:" + Opco + ":" + Pid).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Heading h = obj as Heading;
            if ((object)h == null)
            {
                return false;
            }

            return (h.Opco == Opco && h.Pid == Pid);
        }

        public static Heading FromDictionary(IDictionary<string, Object> dictionary)
        {
            var heading = new Heading();
            heading.Name = dictionary["name"] as string;
            heading.Pid = dictionary["pid"] as string;
            heading.External = dictionary["external"] as string;
            heading.Language = dictionary["language"] as string;
            heading.Opco = dictionary["opco"] as string;
            heading.Version = dictionary["version"] as string;
            return heading;
        }

    }
}
