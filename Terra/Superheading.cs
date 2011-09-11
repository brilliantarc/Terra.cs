using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terra
{
    /// <summary>
    /// Defines a visual grouping of headings.  These superheadings were 
    /// designed to limit the number of headings coming back from the server,
    /// in order to make them easier to work with, like headings grouped
    /// together in rough verticals.
    /// </summary>
    public class Superheading : Node
    {
        /// <summary>
        /// The name of the superheading
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// An SEO-compliant slug for the superheading
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// A third-party identifier for the superheading
        /// </summary>
        public string External { get; set; }

        /// <summary>
        /// The language of the superheading's name
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// The operating company to which this superheading belongs
        /// </summary>
        public string Opco { get; set; }

        /// <summary>
        /// An internal tracking code used to ensure that updates by the client
        /// do not overwrite changes made on the server between the time the
        /// client version of this superheading was retrieved and when it was 
        /// written
        /// </summary>
        public string Version { get; set; }

        public override int GetHashCode()
        {
            return ("superheading:" + Opco + ":" + Slug).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Superheading s = obj as Superheading;
            if ((object)s == null)
            {
                return false;
            }

            return (s.Opco == Opco && s.Slug == Slug);
        }

        public static Superheading FromDictionary(IDictionary<string, Object> dictionary)
        {
            var superheading = new Superheading();
            superheading.Name = dictionary["name"] as string;
            superheading.Slug = dictionary["slug"] as string;
            superheading.External = dictionary["external"] as string;
            superheading.Language = dictionary["language"] as string;
            superheading.Opco = dictionary["opco"] as string;
            superheading.Version = dictionary["version"] as string;
            return superheading;
        }

    }
}
