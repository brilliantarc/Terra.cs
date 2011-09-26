using System;
using System.Collections.Generic;
using RestSharp;

namespace Terra.Service
{
    /// <summary>
    /// Properties represent the relations between an option and a category or
    /// taxonomy.  
    /// <para>
    /// In fact, properties do not truly relate options to categories or
    /// taxonomies.  Rather, the relation between an option and a category
    /// is identified by a type of relation--what we refer to as a "verb".
    /// Associating a property with this relation means creating a property
    /// that has the same slug as this verb.  The property then represents
    /// the relation between the category and option.
    /// </para>
    /// <para>
    /// Associating the property with a category or taxonomy also allows
    /// for a visual indicator to users as to the possible properties used
    /// in that category or taxonomy.  And properties are used as a filter,
    /// to isolate these random verbs from the specific relations used to
    /// model behaviors in Terra, such as "mapped-to" or "child-of".  It
    /// should also be noted these reserved relations are not permitted
    /// to be created as properties.
    /// </para>
    /// <para>
    /// Due to the complex relationship between a Property, a "verb" and
    /// categories, taxonomies, and options, properties may not be deleted.
    /// They may be created and renamed, but not removed.  A future release
    /// may support the removal of properties, but for now the safest course
    /// of action is to simply limit the destruction of properties.
    /// </para>
    /// <para>
    /// Do not create an instance of this directly.  Instead, call in through 
    /// the Terra.Client.Properties.
    /// </para>
    /// </summary>
    /// <seealso cref="Terra.Client.Properties"/>
    public class Properties
    {
        private Client _client;

        public Properties(Client client)
        {
            _client = client;
        }

        /// <summary>
        /// Retrieve all the currently defined properties for the entire
        /// operating company.
        /// </summary>
        /// <param name="opco">The three or four letter code for the opco</param>
        /// <returns>A list of Property objects</returns>
        /// <exception cref="Terra.ServerException">The operating company does not exist</exception>
        public List<Property> All(string opco)
        {
            return _client.Request("properties").AddParameter("opco", opco).MakeRequest<List<Property>>();
        }

        /// <summary>
        /// Look up a property by its slug.  Useful for checking to see if a
        /// property exists.
        /// </summary>
        /// <param name="opco">The three or four letter code for the opco</param>
        /// <param name="slug">The property's slug</param>
        /// <returns>A Property object</returns>
        /// <exception cref="Terra.ServerException">The operating company or property does not exist</exception>
        public Property Get(string opco, string slug)
        {
            return _client.Request("property").AddParameter("opco", opco).AddParameter("slug", slug).MakeRequest<Property>();
        }

        /// <summary>
        /// Create a new property.  Typically a property is created as it is 
        /// needed, and associated with a category or taxonomy so it can be
        /// used to map options to either of those memes.  If you include the
        /// relatedTo, it will automatically map the property to that meme.
        /// <para>
        /// The taxonomy or category to which you are relating this property
        /// must exist in the same operating company as the property.
        /// </para>
        /// <para>
        /// The property's slug will be generated from the name if not otherwise
        /// indicated.
        /// </para>
        /// </summary>
        /// <param name="opco">The three or four letter code for the operating company</param>
        /// <param name="name">The human-readable name for this property</param>
        /// <param name="slug">The relation type, or "verb" for this property</param>
        /// <param name="external">An third-party external identifier for this property (optional)</param>
        /// <param name="language">The two-letter ISO language for the property; defaults to the opco's language</param>
        /// <param name="relatedTo">The category or taxonomy using this property (optional)</param>
        /// <returns>The newly created Property</returns>
        /// <exception cref="Terra.ServerException">
        /// <list type="bullet">
        /// <item>
        /// <description>If any of the parameters submitted are invalid, returns a status of Not Acceptable.</description>
        /// </item>
        /// <item>
        /// <description>If the slug already exists, returns a status of Conflict.</description>
        /// </item>
        /// <item>
        /// <description>If the operating company or relatedTo does not exist, returns a status of Not Found.</description>
        /// </item>
        /// </list>
        /// </exception>
        public Property Create(string opco, string name, string slug = null, string external = null, string language = null, Node relatedTo = null)
        {
            var request = _client.Request("property", Method.POST).
                AddParameter("opco", opco).
                AddParameter("name", name).
                AddParameter("slug", slug).
                AddParameter("external", external).
                AddParameter("lang", language);

            if (relatedTo is Taxonomy)
            {
                request.AddParameter("taxonomy", ((Taxonomy)relatedTo).Slug);
            }
            else if (relatedTo is Category)
            {
                request.AddParameter("category", ((Category)relatedTo).Slug);
            }
                
            return request.MakeRequest<Property>();
        }

        /// <summary>
        /// Update the name, external identifier, or language for this property.  
        /// Neither the operating company nor the slug may be modified.
        /// </summary>
        /// <param name="property">A property with the updated information</param>
        /// <returns>A new Property object, with the updates in place</returns>
        /// <exception cref="Terra.ServerException">
        /// <list type="bullet">
        /// <item>
        /// <description>If any of the parameters submitted are invalid, returns a status of Not Acceptable.</description>
        /// </item>
        /// <item>
        /// <description>If the existing property could not be found on the server, returns a status of Not Found</description>
        /// </item>
        /// <item>
        /// <description>If the local property is older than the version on the server, a Precondition Failed status is returned</description>
        /// </item>
        /// </list>
        /// </exception>
        public Property Update(Property property)
        {
            return _client.Request("property", Method.PUT).
                AddParameter("opco", property.Opco).
                AddParameter("name", property.Name).
                AddParameter("slug", property.Slug).
                AddParameter("external", property.External).
                AddParameter("lang", property.Language).
                AddParameter("v", property.Version).
                MakeRequest<Property>();
        }

        /// <summary>
        /// Get the list of synonyms associated with this property.
        /// </summary>
        /// <param name="property">The property</param>
        /// <returns>A list of synonyms</returns>
        /// <exception cref="Terra.ServerException">The property does not exist</exception>
        public List<Synonym> Synonyms(Property property)
        {
            return _client.Request("property/synonyms").
                AddParameter("opco", property.Opco).
                AddParameter("slug", property.Slug).
                MakeRequest<List<Synonym>>();
        }

        /// <summary>
        /// Create a new synonym and associate it with this property.
        /// <para>
        /// Synonyms may also be used as a simple translation tool.  When you
        /// create a synonym, by default it is assigned to the same langauge as
        /// the operating company.  However, by assigning a different language
        /// to the synonym, you now have a translation for the property.
        /// </para>
        /// </summary>
        /// <param name="property">The property to associate with the synonym</param>
        /// <param name="name">The human-readable name of the synonym</param>
        /// <param name="slug">An SEO-compliant slug for the synonym; generated if not provided</param>
        /// <param name="language">The language of the name; defaults to the opco's language</param>
        /// <returns>The newly created Synonym</returns>
        /// <exception cref="Terra.ServerException">The property does not exist, or the synonym already exists</exception>
        public Synonym CreateSynonym(Property property, string name, string slug = null, string language = null)
        {
            return _client.Request("property/synonym", Method.POST).
                AddParameter("opco", property.Opco).
                AddParameter("name", name).
                AddParameter("slug", slug).
                AddParameter("lang", language).
                AddParameter("property", property.Slug).
                MakeRequest<Synonym>();
        }

        /// <summary>
        /// Associate an existing synonym with a property.
        /// </summary>
        /// <param name="property">The property with which to associate the synonym</param>
        /// <param name="synonym">The synonym for the property</param>
        /// <exception cref="Terra.ServerException">Either the property or the synonym doesn't exist</exception>
        public void AddSynonym(Property property, Synonym synonym)
        {
            _client.Request("property/synonym", Method.PUT).
                AddParameter("opco", property.Opco).
                AddParameter("property", property.Slug).
                AddParameter("slug", synonym.Slug).
                MakeRequest();
        }

        /// <summary>
        /// This is a convenience method to create or add an existing synonym
        /// (or translation) to a property.  If the synonym does not already
        /// exist, it is created with a default slug (and default language, if
        /// not otherwise indicated).  The synonym, existing or new, is
        /// associated with the property.
        /// </summary>
        /// <param name="property">The property to associate with this synonym</param>
        /// <param name="synonym">The new or existing name of a synonym</param>
        /// <param name="language">The language of the synonym; defaults to the opco's language</param>
        /// <returns>The new or existing synonym</returns>
        /// <exception cref="Terra.ServerException">The property does not exist</exception>
        public Synonym AddSynonym(Property property, string synonym, string language = null)
        {
            var slug = _client.Slugify(synonym);
            try
            {
                var existing = _client.Synonyms.Get(property.Opco, slug);
                _client.Request("property/synonym", Method.PUT).
                    AddParameter("opco", property.Opco).
                    AddParameter("property", property.Slug).
                    AddParameter("slug", slug).
                    MakeRequest();
                return existing;
            }
            catch (ServerException e)
            {
                if (e.Status == System.Net.HttpStatusCode.NotFound)
                {
                    return CreateSynonym(property, synonym, slug, language);
                }
                else
                {
                    throw e;
                }
            }
        }

        /// <summary>
        /// Remove the synonym with from the property.  Note that this doesn't
        /// delete the synonym, simply removes its association from this 
        /// property.
        /// </summary>
        /// <param name="property">The property with which to disassociate the synonym</param>
        /// <param name="synonym">The synonym for the property</param>
        /// <exception cref="Terra.ServerException">Either the property or the synonym doesn't exist</exception>
        public void RemoveSynonym(Property property, Synonym synonym)
        {
            _client.Request("property/synonym", Method.DELETE).
                AddParameter("opco", property.Opco).
                AddParameter("property", property.Slug).
                AddParameter("slug", synonym.Slug).
                MakeRequest();
        }
    }
}
