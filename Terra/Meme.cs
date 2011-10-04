using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terra
{
    /// <summary>
    /// A more specific type of node.  Categories, properties, options, headings,
    /// superheadings, and synonyms are all kinds of "memes".  A meme is identified
    /// by its operating company (portfolio), its definition ("Category", "Option"),
    /// and its unique slug.
    /// </summary>
    public interface Meme : Node
    {
        /// <summary>
        /// The three or four letter code for the operating company to which this 
        /// meme belongs.
        /// </summary>
        string Opco { get; }

        /// <summary>
        /// The definition of this meme; used internally.
        /// </summary>
        string Definition { get; }

        /// <summary>
        /// An identifier for the meme, unique to the operating company portfolio.
        /// </summary>
        string Slug { get; }
    }
}
