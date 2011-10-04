using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terra
{
    /// <summary>
    /// A taxonomic category designed around tight ontological principles.  A
    /// category tends to have a stricter definition than an operating company
    /// heading.  Categories are defined by the properties and options associated 
    /// with them, and any heading mapped to a category inherits that category's
    /// properties and options, as well as the properties and options of the
    /// category's parents.
    /// </summary>
    public class Category : Meme
    {
        /// <summary>
        /// The name of the category; need not be unique
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A unique identifier for the category, within the context of the 
        /// operating company
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// A third-party external identifier for the category, perhaps 
        /// referencing back to a unique identifier in another system  Should
        /// be unique in the context of the operating company, but this is
        /// not enforced.
        /// </summary>
        public string External { get; set; }

        /// <summary>
        /// The two-letter ISO language code, indicating the language of the
        /// name of this category
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// A three or four letter code indicating the operating company with
        /// which this category is associated
        /// </summary>
        public string Opco { get; set; }

        /// <summary>
        /// An internal tracking code used to ensure that updates by the client
        /// do not overwrite changes made on the server between the time the
        /// client version of this category was retrieved and when it was 
        /// written
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Returns "Category", which is how Terra defines a category.  Used 
        /// internally; you should not need to refer to this in your client
        /// code.
        /// </summary>
        public string Definition {
            get { return "Category"; }
        }

        public override int GetHashCode()
        {
            return ("category:" + Opco + ":" + Slug).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Category c = obj as Category;
            if ((object)c == null)
            {
                return false;
            }

            return (c.Opco == Opco && c.Slug == Slug);
        }

        public static Category FromDictionary(IDictionary<string, Object> dictionary)
        {
            var category = new Category();
            category.Name = dictionary["name"] as string;
            category.Slug = dictionary["slug"] as string;
            category.External = dictionary["external"] as string;
            category.Language = dictionary["language"] as string;
            category.Opco = dictionary["opco"] as string;
            category.Version = dictionary["version"] as string;
            return category;
        }
    }
}
