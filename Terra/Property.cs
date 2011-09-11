using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terra
{
    /// <summary>
    /// Relates an option to a category in a meaningful way.  For example, a
    /// property might be "Cuisine", and tie "Steak" to a restaurant, while
    /// another property "Sells" may associate "Steak" with a butcher.  It
    /// is the "verb" in any category-option relation.
    /// <para>
    /// The Property class itself represents a meme that internally is not
    /// really the relation between a category or taxonomy and an option.
    /// Instead, it provides a human-friendly form of that relation's verb.
    /// The relation itself is isolated from the Property meme deep inside
    /// of Terra.  In most instances you can treat the Property as the
    /// relation, but know there are also relations between objects in 
    /// Terra which are not defined as properties.
    /// </para>
    /// </summary>
    public class Property : Node
    {
        /// <summary>
        /// A human-readable name for the property
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The relation type ("verb") used to indicate this property relation
        /// between an option and a category or taxonomy
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// A third-party external identifier for the property; optional, and
        /// rare
        /// </summary>
        public string External { get; set; }

        /// <summary>
        /// The language of the property name
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// The operating company for which this property is associated
        /// </summary>
        public string Opco { get; set; }

        /// <summary>
        /// An internal tracking code used to ensure that updates by the client
        /// do not overwrite changes made on the server between the time the
        /// client version of this property was retrieved and when it was 
        /// written
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// When requesting properties and options for a category, taxonomy, or
        /// heading, the options will return associated with each property
        /// </summary>
        public List<Option> Options { get; set; }

        public override bool Equals(object obj)
        {
            Property p = obj as Property;
            if ((object)p == null)
            {
                return false;
            }

            return (p.Opco == Opco && p.Slug == Slug);
        }

        public static Property FromDictionary(IDictionary<string, Object> dictionary)
        {
            var property = new Property();
            property.Name = dictionary["name"] as string;
            property.Slug = dictionary["slug"] as string;
            property.External = dictionary["external"] as string;
            property.Language = dictionary["language"] as string;
            property.Opco = dictionary["opco"] as string;
            property.Version = dictionary["version"] as string;

            if (dictionary.ContainsKey("options"))
            {
                property.Options = new List<Option>();
                foreach (var option in (IList<IDictionary<string, Object>>)dictionary["options"])
                {
                    property.Options.Add(Option.FromDictionary(option));
                }
            }

            return property;
        }
    }
}
