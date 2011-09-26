using System.Collections.Generic;
using RestSharp;

namespace Terra.Service
{
    /// <summary>
    /// Service API calls relating to taxonomies.  
    /// <para>
    /// Do not create an instance of this directly.  Instead, call in through 
    /// the Terra.Client.Taxonomies.
    /// </para>
    /// </summary>
    /// <seealso cref="Terra.Client.Taxonomies"/>
    public class Taxonomies
    {
        private Client _client;
        
        public Taxonomies(Client client)
        {
            _client = client;
        }

        /// <summary>
        /// Look up the taxonomies currently available for the given operating 
        /// company.
        /// </summary>
        /// <param name="opco">The three or four letter code for the operating company, e.g. "PKT"</param>
        /// <returns>A list of Taxonomy object</returns>
        /// <exception cref="Terra.ServerException">The operating company suggested does not exist</exception>
        public List<Taxonomy> All(string opco)
        {
            return _client.Request("taxonomies").AddParameter("opco", opco).MakeRequest<List<Taxonomy>>();
        }

        /// <summary>
        /// Look for details about a specific taxonomy.  Note that this call 
        /// simply returns the basic information, such as the name and language
        /// of the taxonomy.  It does not return child categories or mappings.
        /// 
        /// It is useful for confirming the existence of a taxonomy.
        /// </summary>
        /// <param name="opco">The three or four letter code for the operating company, e.g. "PKT"</param>
        /// <param name="slug">The slug identifier for the taxonomy, unique to the operating company</param>
        /// <returns>A Taxonomy</returns>
        /// <exception cref="Terra.ServerException">Either the operating company or taxonomy does not exist</exception>
        public Taxonomy Get(string opco, string slug)
        {
            return _client.Request("taxonomy").AddParameter("opco", opco).AddParameter("slug", slug).MakeRequest<Taxonomy>();
        }

        /// <summary>
        /// Create a new taxonomy for the given operating company.  The taxonomy
        /// name need not be unique, but its (optional) slug must be.  
        /// 
        /// If the slug is not provided (either null or an empty string), it will
        /// be created by transforming the name of the taxonomy into an SEO-ready
        /// value.  For example, the name "Travel, Accommodations, and Food 
        /// Services" would be transformed into "travel-accommodation-and-food-services".
        /// 
        /// If language is not supplied it will default to the language of the
        /// operating company.
        /// </summary>
        /// <param name="opco">The three or four letter code for the operating company</param>
        /// <param name="name">The name of the taxonomy</param>
        /// <param name="slug">A unique identifier for the taxonomy, or null to have one generated</param>
        /// <param name="language">The language of the taxonomy, or null to use the opco's language</param>
        /// <returns>The newly created Taxonomy object</returns>
        /// <exception cref="Terra.ServerException">
        /// <list type="bullet">
        /// <item>
        /// <description>If any of the parameters submitted are invalid, returns a status of Not Acceptable.</description>
        /// </item>
        /// <item>
        /// <description>If the slug already exists, returns a status of Conflict.</description>
        /// </item>
        /// <item>
        /// <description>If the operating company does not exist, returns a status of Not Found.</description>
        /// </item>
        /// </list>
        /// </exception>
        public Taxonomy Create(string opco, string name, string slug = null, string language = null)
        {
            return _client.Request("taxonomy", Method.POST).
                AddParameter("opco", opco).
                AddParameter("name", name).
                AddParameter("slug", slug).
                AddParameter("lang", language).
                MakeRequest<Taxonomy>();
        }

        /// <summary>
        /// Update's the name and language of the given Taxonomy with the server. 
        /// Will not change the slug or operating company however.  If you modify
        /// either of those, mostly likely you will receive a Not Found status in
        /// the ServerException, indicating the Taxonomy object could not be found
        /// on the Terra server.
        /// 
        /// Note that this method will return a new Taxonomy object.  The original
        /// object will not be modified.
        /// </summary>
        /// <param name="taxonomy">An existing taxonomy with its name or language modified</param>
        /// <returns>A new Taxonomy object with the updated information, as confirmation</returns>
        /// <exception cref="Terra.ServerException">
        /// <list type="bullet">
        /// <item>
        /// <description>If any of the parameters submitted are invalid, returns a status of Not Acceptable.</description>
        /// </item>
        /// <item>
        /// <description>If the existing taxonomy could not be found on the server, returns a status of Not Found</description>
        /// </item>
        /// <item>
        /// <description>If the local taxonomy is older than the version on the server, a Precondition Failed status is returned</description>
        /// </item>
        /// </list>
        /// </exception>
        public Taxonomy Update(Taxonomy taxonomy)
        {
            return _client.Request("taxonomy", Method.PUT).
                AddParameter("opco", taxonomy.Opco).
                AddParameter("name", taxonomy.Name).
                AddParameter("slug", taxonomy.Slug).
                AddParameter("lang", taxonomy.Language).
                AddParameter("v", taxonomy.Version).
                MakeRequest<Taxonomy>();
        }

        /// <summary>
        /// Completely deletes the given taxonomy from the Terra server.  If
        /// there are any categories associated with the taxonomy, they are
        /// orphaned, as are any properties and options associated with the
        /// taxonomy.
        /// 
        /// This method returns nothing.  If the delete is unsuccessful, an
        /// exception will be raised.  Otherwise it completed successfully.
        /// </summary>
        /// <param name="taxonomy">The taxonomy to delete; only Opco and Slug are used</param>
        /// <exception cref="Terra.ServerException">
        /// Raises a Not Found exception if the taxonomy does not exist, or
        /// a Precondition Failed if the taxonomy on the server is newer than
        /// the one submitted.
        /// </exception>
        public void Delete(Taxonomy taxonomy)
        {
            _client.Request("taxonomy", Method.DELETE).
                AddParameter("opco", taxonomy.Opco).
                AddParameter("slug", taxonomy.Slug).
                AddParameter("v", taxonomy.Version).
                MakeRequest();
        }

        /// <summary>
        /// Get the list of synonyms associated with this taxonomy.
        /// </summary>
        /// <param name="taxonomy">The taxonomy</param>
        /// <returns>A list of synonyms</returns>
        /// <exception cref="Terra.ServerException">The taxonomy does not exist</exception>
        public List<Synonym> Synonyms(Taxonomy taxonomy)
        {
            return _client.Request("taxonomy/synonyms").
                AddParameter("opco", taxonomy.Opco).
                AddParameter("slug", taxonomy.Slug).
                MakeRequest<List<Synonym>>();
        }

        /// <summary>
        /// Create a new synonym and associate it with this taxonomy.
        /// 
        /// Synonyms may also be used as a simple translation tool.  When you
        /// create a synonym, by default it is assigned to the same langauge as
        /// the operating company.  However, by assigning a different language
        /// to the synonym, you now have a translation for the taxonomy.
        /// </summary>
        /// <param name="taxonomy">The taxonomy to associate with the synonym</param>
        /// <param name="name">The human-readable name of the synonym</param>
        /// <param name="slug">An SEO-compliant slug for the synonym; generated if not provided</param>
        /// <param name="language">The language of the name; defaults to the opco's language</param>
        /// <returns>The newly created Synonym</returns>
        /// <exception cref="Terra.ServerException">The taxonomy does not exist, or the synonym already exists</exception>
        public Synonym CreateSynonym(Taxonomy taxonomy, string name, string slug = null, string language = null)
        {
            return _client.Request("taxonomy/synonym", Method.POST).
                AddParameter("opco", taxonomy.Opco).
                AddParameter("name", name).
                AddParameter("slug", slug).
                AddParameter("lang", language).
                AddParameter("taxonomy", taxonomy.Slug).
                MakeRequest<Synonym>();
        }

        /// <summary>
        /// Associate an existing synonym with a taxonomy.
        /// </summary>
        /// <param name="taxonomy">The taxonomy with which to associate the synonym</param>
        /// <param name="synonym">The synonym for the taxonomy</param>
        /// <exception cref="Terra.ServerException">Either the taxonomy or the synonym doesn't exist</exception>
        public void AddSynonym(Taxonomy taxonomy, Synonym synonym)
        {
            _client.Request("taxonomy/synonym", Method.PUT).
                AddParameter("opco", taxonomy.Opco).
                AddParameter("taxonomy", taxonomy.Slug).
                AddParameter("slug", synonym.Slug).
                MakeRequest();
        }

        /// <summary>
        /// This is a convenience method to create or add an existing synonym
        /// (or translation) to a taxonomy.  If the synonym does not already
        /// exist, it is created with a default slug (and default language, if
        /// not otherwise indicated).  The synonym, existing or new, is
        /// associated with the taxonomy.
        /// </summary>
        /// <param name="taxonomy">The taxonomy to associate with this synonym</param>
        /// <param name="synonym">The new or existing name of a synonym</param>
        /// <param name="language">The language of the synonym; defaults to the opco's language</param>
        /// <returns>The new or existing synonym</returns>
        /// <exception cref="Terra.ServerException">The taxonomy does not exist</exception>
        public Synonym AddSynonym(Taxonomy taxonomy, string synonym, string language = null)
        {
            var slug = _client.Slugify(synonym);
            try
            {
                var existing = _client.Synonyms.Get(taxonomy.Opco, slug);
                _client.Request("taxonomy/synonym", Method.PUT).
                    AddParameter("opco", taxonomy.Opco).
                    AddParameter("taxonomy", taxonomy.Slug).
                    AddParameter("slug", slug).
                    MakeRequest();
                return existing;
            }
            catch (ServerException e)
            {
                if (e.Status == System.Net.HttpStatusCode.NotFound)
                {
                    return CreateSynonym(taxonomy, synonym, slug, language);
                }
                else
                {
                    throw e;
                }
            }
        }

        /// <summary>
        /// Remove the synonym with from the taxonomy.  Note that this doesn't
        /// delete the synonym, simply removes its association from this 
        /// taxonomy.
        /// </summary>
        /// <param name="taxonomy">The taxonomy with which to disassociate the synonym</param>
        /// <param name="synonym">The synonym for the taxonomy</param>
        /// <exception cref="Terra.ServerException">Either the taxonomy or the synonym doesn't exist</exception>
        public void RemoveSynonym(Taxonomy taxonomy, Synonym synonym)
        {
            _client.Request("taxonomy/synonym", Method.DELETE).
                AddParameter("opco", taxonomy.Opco).
                AddParameter("taxonomy", taxonomy.Slug).
                AddParameter("slug", synonym.Slug).
                MakeRequest();
        }

        /// <summary>
        /// Get the top-level categories for the given taxonomy.  Note that this
        /// does not return all the categories for a taxonomy.
        /// 
        /// This is the same call present in the Terra.Service.Categories class.
        /// It is here as a convenience.
        /// </summary>
        /// <seealso cref="Terra.Service.Categories.Children(Terra.Category)"/>
        /// <param name="taxonomy">The taxonomy of categories</param>
        /// <returns>A list of Category objects</returns>
        /// <exception cref="Terra.ServerException">The given taxonomy does not exist</exception>
        public List<Category> Children(Taxonomy taxonomy)
        {
            return _client.Categories.Children(taxonomy);
        }

        /// <summary>
        /// Look up the properties associated with the given taxonomy.  The list
        /// returned does not include options, and there may be properties in
        /// the list that have no options associated with them anyway.
        /// </summary>
        /// <param name="taxonomy">The taxonomy with which the properties are associated</param>
        /// <returns>A list of Property objects</returns>
        /// <exception cref="Terra.ServerException">The taxonomy does not exist</exception>
        public List<Property> Properties(Taxonomy taxonomy)
        {
            return _client.Request("taxonomy/properties").
                AddParameter("opco", taxonomy.Opco).
                AddParameter("slug", taxonomy.Slug).
                MakeRequest<List<Property>>();
        }

        /// <summary>
        /// Associate a property with a taxonomy.  This let's both users and 
        /// the software know what properties should be included in queries 
        /// such as inheritance.  It also serves as a visual tool for users, 
        /// so they know the list of possible properties from which to select 
        /// when adding options.
        /// </summary>
        /// <param name="taxonomy">The taxonomy to which to add the property</param>
        /// <param name="property">The property to associate</param>
        /// <exception cref="Terra.ServerException">Either the property or the taxonomy doesn't exist</exception>
        public void AddProperty(Taxonomy taxonomy, Property property)
        {
            _client.Request("taxonomy/property", Method.PUT).
                AddParameter("opco", taxonomy.Opco).
                AddParameter("taxonomy", taxonomy.Slug).
                AddParameter("property", property.Slug).
                MakeRequest();
        }

        /// <summary>
        /// Remove a property from a taxonomy.  Note that this does not remove 
        /// any of the relations between options and this node.  It does, 
        /// however, filter the options from any requests that use the properties 
        /// associated with the node as a filter, such as inheritance.
        /// </summary>
        /// <param name="taxonomy">The taxonomy from which to remove the property</param>
        /// <param name="property">The property to remove</param>
        /// <exception cref="Terra.ServerException">Either the property or the taxonomy doesn't exist</exception>
        public void RemoveProperty(Taxonomy taxonomy, Property property)
        {
            _client.Request("taxonomy/property", Method.DELETE).
                AddParameter("opco", taxonomy.Opco).
                AddParameter("taxonomy", taxonomy.Slug).
                AddParameter("property", property.Slug).
                MakeRequest();
        }

        /// <summary>
        /// Find all the options directly associated with this taxonomy.  
        /// 
        /// This operation can be a bit slow, as the system has to filter and 
        /// collate options and properties.  It is typically faster to request
        /// a list of properties, then request the options for a selected
        /// property (this is how the Terra UI does things).  Of course by 
        /// "slow", we mean takes around 300ms.
        /// </summary>
        /// <param name="taxonomy">The taxonomy of options to retrieve</param>
        /// <returns>A list of Property objects with their Options list filled</returns>
        /// <exception cref="Terra.ServerException">The taxonomy doesn't exist</exception>
        public List<Property> Options(Taxonomy taxonomy)
        {
            return _client.Request("taxonomy/options").
                AddParameter("opco", taxonomy.Opco).
                AddParameter("slug", taxonomy.Slug).
                GenericRequest().ConvertAll<Property>(node => (Property) node);;
        }

        /// <summary>
        /// Find all the options directly associated with this category by the 
        /// given property.  
        /// </summary>
        /// <param name="taxonomy">The taxonomy of options to retrieve</param>
        /// <param name="property">The property to limit the options by</param>
        /// <returns>A list of the Options associated with the taxonomy via the property</returns>
        public List<Option> Options(Taxonomy taxonomy, Property property)
        {
            return _client.Request("taxonomy/options").
                AddParameter("opco", taxonomy.Opco).
                AddParameter("slug", taxonomy.Slug).
                AddParameter("property", property.Slug).
                MakeRequest<List<Option>>();
        }

        /// <summary>
        /// Add an option to a taxonomy.  The taxonomy, option, and property
        /// must all exist in the same operating company.
        /// </summary>
        /// <param name="taxonomy">The taxonomy to associate the option</param>
        /// <param name="property">The property or "verb" used in the relation</param>
        /// <param name="option">The option being related</param>
        /// <exception cref="Terra.ServerException">Either the option, property or the taxonomy doesn't exist</exception>
        public void AddOption(Taxonomy taxonomy, Property property, Option option)
        {
            _client.Request("taxonomy/option", Method.PUT).
                AddParameter("opco", taxonomy.Opco).
                AddParameter("taxonomy", taxonomy.Slug).
                AddParameter("property", property.Slug).
                AddParameter("option", option.Slug).
                MakeRequest();
        }

        /// <summary>
        /// Remove an option from a taxonomy.  The taxonomy, option, and property
        /// must all exist in the same operating company.
        /// </summary>
        /// <param name="taxonomy">The taxonomy from which to remove the option</param>
        /// <param name="property">The property or "verb" used in the relation</param>
        /// <param name="option">The option being removed</param>
        /// <exception cref="Terra.ServerException">Either the option, property or the taxonomy doesn't exist</exception>
        public void RemoveOption(Taxonomy taxonomy, Property property, Option option)
        {
            _client.Request("taxonomy/option", Method.DELETE).
                AddParameter("opco", taxonomy.Opco).
                AddParameter("taxonomy", taxonomy.Slug).
                AddParameter("property", property.Slug).
                AddParameter("option", option.Slug).
                MakeRequest();
        }
    }
}
