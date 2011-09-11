using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terra
{
    /// <summary>
    /// An idea or keyword associated with one or more categories via a
    /// property.  The combination of an option and its property provide
    /// a meaningful piece of information about a category, which is then
    /// inherited by any child categories or mapped headings.  
    /// <para>
    /// While an option's slug must be unique to an operating company, its
    /// name need not be.  This can be used to keep an option distinct in
    /// meaning that shares common spelling with other options (homynyms).
    /// For example, two options may be named "Helmet", while one has the
    /// slug "bicycle-helmet" and the other has the slug "motorcycle-helmet".
    /// </para>
    /// </summary>
    public class Option : Node
    {
        /// <summary>
        /// A name for this option; need not be unique
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// An SEO-compliant slug for the option; must be unique in the context
        /// of an operating company
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// A third-party external identifier for the option; we try to keep
        /// this unique, but it is not required nor enforced by Terra
        /// </summary>
        public string External { get; set; }

        /// <summary>
        /// The language of the option name
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// The operating company with which this option is associated
        /// </summary>
        public string Opco { get; set; }

        /// <summary>
        /// An internal tracking code used to ensure that updates by the client
        /// do not overwrite changes made on the server between the time the
        /// client version of this option was retrieved and when it was 
        /// written
        /// </summary>
        public string Version { get; set; }

        public override bool Equals(object obj)
        {
            Option o = obj as Option;
            if ((object)o == null)
            {
                return false;
            }

            return (o.Opco == Opco && o.Slug == Slug);
        }

        public static Option FromDictionary(IDictionary<string, Object> dictionary)
        {
            var option = new Option();
            option.Name = dictionary["name"] as string;
            option.Slug = dictionary["slug"] as string;
            option.External = dictionary["external"] as string;
            option.Language = dictionary["language"] as string;
            option.Opco = dictionary["opco"] as string;
            option.Version = dictionary["version"] as string;
            return option;
        }
    }
}
