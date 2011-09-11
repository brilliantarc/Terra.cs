using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terra
{
    /// <summary>
    /// An secondary, alternate term for any piece of information in Terra, be 
    /// it a category, property, option, or taxonomy.  Even headings and 
    /// superheadings may have synonyms.  
    /// <para>
    /// Synonyms may also be used as simple translations of a term, given that
    /// the synonym's language may freely differ from that of the meme to which
    /// it is related.  Ideally, it is better, for example, to map a category in
    /// one language to its equivalent in another, but for simple uses a synonym
    /// may suffice.
    /// </para>
    /// </summary>
    public class Synonym : Node
    {
        /// <summary>
        /// The name of this synonym
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// An SEO-compliant slug for the synonym
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// An external identifier for the synonym (rare)
        /// </summary>
        public string External { get; set; }

        /// <summary>
        /// The language of the synonym name
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// The operating company responsible for this synonym
        /// </summary>
        public string Opco { get; set; }

        /// <summary>
        /// An internal tracking code used to ensure that updates by the client
        /// do not overwrite changes made on the server between the time the
        /// client version of this synonym was retrieved and when it was 
        /// written
        /// </summary>
        public string Version { get; set; }

        public override int GetHashCode()
        {
            return ("synonym:" + Opco + ":" + Slug).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Synonym s = obj as Synonym;
            if ((object)s == null)
            {
                return false;
            }

            return (s.Opco == Opco && s.Slug == Slug);
        }

        public static Synonym FromDictionary(IDictionary<string, Object> dictionary)
        {
            var synonym = new Synonym();
            synonym.Name = dictionary["name"] as string;
            synonym.Slug = dictionary["slug"] as string;
            synonym.External = dictionary["external"] as string;
            synonym.Language = dictionary["language"] as string;
            synonym.Opco = dictionary["opco"] as string;
            synonym.Version = dictionary["version"] as string;
            return synonym;
        }

    }
}
