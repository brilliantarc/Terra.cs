using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terra
{
    /// <summary>
    /// A collection of categories within an operating company.  Taxonomies
    /// may act as categories in many situations as well.  For example, a
    /// taxonomy may have properties and options associated with it.  Any
    /// options associated with a taxonomy will be inherited by all of the
    /// categories in the taxonomy.
    /// </summary>
    public class Taxonomy : Node
    {
        /// <summary>
        /// The name of the taxonomy
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// An identifier for the taxonomy, unique in the context of the 
        /// operating company
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// The default language for the taxonomy
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// The code for the operating company with which this taxonomy is 
        /// associated
        /// </summary>
        public string Opco { get; set; }

        /// <summary>
        /// Used internally by the server to verify the information local to 
        /// the client is not out of date with what is on the server.  If 
        /// version values on the client and server do not match when a taxonomy
        /// is modified, an exception will be thrown.
        /// </summary>
        public string Version { get; set; }

        public override int GetHashCode()
        {
            return ("taxonomy:" + Opco + ":" + Slug).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Taxonomy t = obj as Taxonomy;
            if ((object)t == null)
            {
                return false;
            }

            return (t.Opco == Opco && t.Slug == Slug);
        }

        public static Taxonomy FromDictionary(IDictionary<string, Object> dictionary)
        {
            var taxonomy = new Taxonomy();
            taxonomy.Name = dictionary["name"] as string;
            taxonomy.Slug = dictionary["slug"] as string;
            taxonomy.Language = dictionary["language"] as string;
            taxonomy.Opco = dictionary["opco"] as string;
            taxonomy.Version = dictionary["version"] as string;
            return taxonomy;
        }


    }
}
