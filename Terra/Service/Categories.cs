using System;
using System.Collections.Generic;
using RestSharp;

namespace Terra.Service
{
    /// <summary>
    /// Terra API calls relating to categories.  
    /// <para>
    /// Do not create an instance of this directly.  Instead, call in through 
    /// the Terra.Client.Categories.
    /// </para>
    /// </summary>
    /// <seealso cref="Terra.Client.Categories"/>
    public class Categories
    {
        private Client _client;

        public Categories(Client client)
        {
            _client = client;
        }

        /// <summary>
        /// Get the top-level categories for the given taxonomy.  Note that this
        /// does not return all the categories for a taxonomy.
        /// </summary>
        /// <param name="taxonomy">The taxonomy of categories</param>
        /// <returns>A list of Category objects</returns>
        /// <exception cref="Terra.ServerException">The given taxonomy does not exist</exception>
        public List<Category> Children(Taxonomy taxonomy)
        {
            return _client.Request("taxonomy/categories").
                AddParameter("opco", taxonomy.Opco).
                AddParameter("slug", taxonomy.Slug).
                MakeRequest<List<Category>>();
        }

        /// <summary>
        /// Look for details about a specific category.  Note that this call 
        /// simply returns the basic information, such as the name and language
        /// of the category.  It does not return child categories or mappings.
        /// <para>
        /// It is useful for confirming the existence of a category.
        /// </para>
        /// </summary>
        /// <param name="opco">The three or four letter code for the operating company, e.g. "PKT"</param>
        /// <param name="slug">The slug identifier for the category, unique to the operating company</param>
        /// <returns>A category</returns>
        /// <exception cref="Terra.ServerException">Either the operating company or category does not exist</exception>
        public Category Get(string opco, string slug)
        {
            return _client.Request("category").
                AddParameter("opco", opco).
                AddParameter("slug", slug).
                MakeRequest<Category>();
        }

        /// <summary>
        /// Create a new category for the given operating company.  The category
        /// name need not be unique, but its (optional) slug must be.  
        /// <para>
        /// If the slug is not provided (either null or an empty string), it will
        /// be created by transforming the name of the category into an SEO-ready
        /// value.  For example, the name "Mexican Restaurants" would be transformed 
        /// into "mexican-restaurants".
        /// </para>
        /// <para>
        /// If language is not supplied it will default to the language of the
        /// operating company.
        /// </para>
        /// </summary>
        /// <param name="opco">The three or four letter code for the operating company</param>
        /// <param name="name">The name of the category</param>
        /// <param name="slug">A unique identifier for the category, or null to have one generated</param>
        /// <param name="language">The language of the category, or null to use the opco's language</param>
        /// <param name="parent">May be either a category or a taxonomy; if a taxonomy, becomes a top-level category in that taxonomy</param>
        /// <returns>The newly created Category object</returns>
        /// <exception cref="Terra.ServerException">
        /// <list type="bullet">
        /// <item>
        /// <description>If any of the parameters submitted are invalid, returns a status of Not Acceptable.</description>
        /// </item>
        /// <item>
        /// <description>If the slug already exists, returns a status of Conflict.</description>
        /// </item>
        /// <item>
        /// <description>If the operating company or parent does not exist, returns a status of Not Found.</description>
        /// </item>
        /// </list>
        /// </exception>
        public Category Create(string opco, string name, string slug = null, string language = null, Node parent = null)
        {
            var request = _client.Request("category", Method.POST).
                AddParameter("opco", opco).
                AddParameter("name", name).
                AddParameter("slug", slug).
                AddParameter("lang", language);

            if (parent is Taxonomy)
            {
                request.AddParameter("taxonomy", ((Taxonomy)parent).Slug);
            }
            else if (parent is Category)
            {
                request.AddParameter("category", ((Category)parent).Slug);
            }

            return request.MakeRequest<Category>();
        }

        /// <summary>
        /// Update's the name and language of the given category with the server. 
        /// Will not change the slug or operating company however.  If you modify
        /// either of those, mostly likely you will receive a Not Found status in
        /// the ServerException, indicating the category object could not be found
        /// on the Terra server.
        /// <para>
        /// Note that this method will return a new Category object.  The original
        /// object will not be modified.
        /// </para>
        /// </summary>
        /// <param name="category">An existing category with its name or language modified</param>
        /// <returns>A new Category object with the updated information, as confirmation</returns>
        /// <exception cref="Terra.ServerException">
        /// <list type="bullet">
        /// <item>
        /// <description>If any of the parameters submitted are invalid, returns a status of Not Acceptable.</description>
        /// </item>
        /// <item>
        /// <description>If the existing category could not be found on the server, returns a status of Not Found</description>
        /// </item>
        /// <item>
        /// <description>If the local category is older than the version on the server, a Precondition Failed status is returned</description>
        /// </item>
        /// </list>
        /// </exception>
        public Category Update(Category category)
        {
            return _client.Request("category", Method.PUT).
                AddParameter("opco", category.Opco).
                AddParameter("name", category.Name).
                AddParameter("slug", category.Slug).
                AddParameter("lang", category.Language).
                AddParameter("v", category.Version).
                MakeRequest<Category>();
        }

        /// <summary>
        /// Completely deletes the given category from the Terra server.  If
        /// there are any categories associated with the category, they are
        /// orphaned, as are any properties and options associated with the
        /// category.
        /// <para>
        /// This method returns nothing.  If the delete is unsuccessful, an
        /// exception will be raised.  Otherwise it completed successfully.
        /// </para>
        /// </summary>
        /// <param name="category">The category to delete; only Opco and Slug are used</param>
        /// <exception cref="Terra.ServerException">
        /// Raises a Not Found exception if the category does not exist, or
        /// a Precondition Failed if the category on the server is newer than
        /// the one submitted.
        /// </exception>
        public void Delete(Category category)
        {
            _client.Request("category", Method.DELETE).
                AddParameter("opco", category.Opco).
                AddParameter("slug", category.Slug).
                AddParameter("v", category.Version).
                MakeRequest();
        }

        /// <summary>
        /// Get the list of synonyms associated with this category.
        /// </summary>
        /// <param name="category">The category</param>
        /// <returns>A list of synonyms</returns>
        /// <exception cref="Terra.ServerException">The category does not exist</exception>
        public List<Synonym> Synonyms(Category category)
        {
            return _client.Request("category/synonyms").
                AddParameter("opco", category.Opco).
                AddParameter("slug", category.Slug).
                MakeRequest<List<Synonym>>();
        }

        /// <summary>
        /// Create a new synonym and associate it with this category.
        /// <para>
        /// Synonyms may also be used as a simple translation tool.  When you
        /// create a synonym, by default it is assigned to the same langauge as
        /// the operating company.  However, by assigning a different language
        /// to the synonym, you now have a translation for the category.
        /// </para>
        /// </summary>
        /// <param name="category">The category to associate with the synonym</param>
        /// <param name="name">The human-readable name of the synonym</param>
        /// <param name="slug">An SEO-compliant slug for the synonym; generated if not provided</param>
        /// <param name="language">The language of the name; defaults to the opco's language</param>
        /// <returns>The newly created Synonym</returns>
        /// <exception cref="Terra.ServerException">The category does not exist, or the synonym already exists</exception>
        public Synonym CreateSynonym(Category category, string name, string slug = null, string language = null)
        {
            return _client.Request("category/synonym", Method.POST).
                AddParameter("opco", category.Opco).
                AddParameter("name", name).
                AddParameter("slug", slug).
                AddParameter("lang", language).
                AddParameter("category", category.Slug).
                MakeRequest<Synonym>();
        }

        /// <summary>
        /// Associate an existing synonym with a category.
        /// </summary>
        /// <param name="category">The category with which to associate the synonym</param>
        /// <param name="synonym">The synonym for the category</param>
        /// <exception cref="Terra.ServerException">Either the category or the synonym doesn't exist</exception>
        public void AddSynonym(Category category, Synonym synonym)
        {
            _client.Request("category/synonym", Method.PUT).
                AddParameter("opco", category.Opco).
                AddParameter("category", category.Slug).
                AddParameter("slug", synonym.Slug).
                MakeRequest();
        }

        /// <summary>
        /// This is a convenience method to create or add an existing synonym
        /// (or translation) to a category.  If the synonym does not already
        /// exist, it is created with a default slug (and default language, if
        /// not otherwise indicated).  The synonym, existing or new, is
        /// associated with the category.
        /// </summary>
        /// <param name="category">The category to associate with this synonym</param>
        /// <param name="synonym">The new or existing name of a synonym</param>
        /// <param name="language">The language of the synonym; defaults to the opco's language</param>
        /// <returns>The new or existing synonym</returns>
        /// <exception cref="Terra.ServerException">The category does not exist</exception>
        public Synonym AddSynonym(Category category, string synonym, string language = null)
        {
            var slug = _client.Slugify(synonym);
            try
            {
                var existing = _client.Synonyms.Get(category.Opco, slug);
                _client.Request("category/synonym", Method.PUT).
                    AddParameter("opco", category.Opco).
                    AddParameter("category", category.Slug).
                    AddParameter("slug", slug).
                    MakeRequest();
                return existing;
            }
            catch (ServerException e)
            {
                if (e.Status == System.Net.HttpStatusCode.NotFound)
                {
                    return CreateSynonym(category, synonym, slug, language);
                }
                else
                {
                    throw e;
                }
            }
        }

        /// <summary>
        /// Remove the synonym with from the category.  Note that this doesn't
        /// delete the synonym, simply removes its association from this 
        /// category.
        /// </summary>
        /// <param name="category">The category with which to disassociate the synonym</param>
        /// <param name="synonym">The synonym for the category</param>
        /// <exception cref="Terra.ServerException">Either the category or the synonym doesn't exist</exception>
        public void RemoveSynonym(Category category, Synonym synonym)
        {
            _client.Request("category/synonym", Method.DELETE).
                AddParameter("opco", category.Opco).
                AddParameter("category", category.Slug).
                AddParameter("slug", synonym.Slug).
                MakeRequest();
        }

        /// <summary>
        /// Get the direct child categories of the given category.
        /// </summary>
        /// <param name="category">The parent category</param>
        /// <returns>A list of Category objects, or an empty list if there are none</returns>
        /// <exception cref="Terra.ServerException">The given category</exception>
        public List<Category> Children(Category category)
        {
            return _client.Request("category/children").
                AddParameter("opco", category.Opco).
                AddParameter("slug", category.Slug).
                MakeRequest<List<Category>>();
        }

        /// <summary>
        /// Get the parent categories or taxonomies for the given category.
        /// </summary>
        /// <param name="category">The child category</param>
        /// <returns>A mixed list of Category and Taxonomy objects</returns>
        public List<Node> Parents(Category category)
        {
            return _client.Request("category/parents").
                AddParameter("opco", category.Opco).
                AddParameter("slug", category.Slug).
                GenericRequest();
        }

        /// <summary>
        /// Make the child category a child of the parent category.  
        /// <para>
        /// Please be aware that this will not automatically update any local 
        /// lists or caches that you may have; the complexity of updating that 
        /// information is left up to the client software.  
        /// </para>
        /// <para>
        /// This method will not return any information upon success.  If it
        /// fails for any reason, a Terra.ServerException is thrown.
        /// </para>
        /// </summary>
        /// <param name="parent">The parent category</param>
        /// <param name="child">The child category</param>
        /// <exception cref="Terra.ServerException">Either the parent or child doesn't exist, or they cannot be related</exception>
        public void AddChild(Category parent, Category child)
        {
            _client.Request("category/children", Method.PUT).
                AddParameter("opco", parent.Opco).
                AddParameter("parent", parent.Slug).
                AddParameter("child", child.Slug).
                MakeRequest();
        }

        /// <summary>
        /// Remove the child category from the parent category.  
        /// <para>
        /// Please be aware that this will not automatically update any local 
        /// lists or caches that you may have; the complexity of updating that 
        /// information is left up to the client software.  
        /// </para>
        /// <para>
        /// This method will not return any information upon success.  If it
        /// fails for any reason, a Terra.ServerException is thrown.
        /// </para>
        /// </summary>
        /// <param name="parent">The parent category</param>
        /// <param name="child">The child category</param>
        /// <exception cref="Terra.ServerException">Either the parent or child doesn't exist, or their relation cannot be broken</exception>
        public void RemoveChild(Category parent, Category child)
        {
            _client.Request("category/children", Method.DELETE).
                AddParameter("opco", parent.Opco).
                AddParameter("parent", parent.Slug).
                AddParameter("child", child.Slug).
                MakeRequest();
        }

        /// <summary>
        /// Look up the properties associated with the given category.  The list
        /// returned does not include options, and there may be properties in
        /// the list that have no options associated with them anyway.
        /// </summary>
        /// <param name="category">The category with which the properties are associated</param>
        /// <returns>A list of Property objects</returns>
        /// <exception cref="Terra.ServerException">The category does not exist</exception>
        public List<Property> Properties(Category category)
        {
            return _client.Request("category/properties").
                AddParameter("opco", category.Opco).
                AddParameter("slug", category.Slug).
                MakeRequest<List<Property>>();
        }

        /// <summary>
        /// Associate a property with a category.  This let's both users and 
        /// the software know what properties should be included in queries 
        /// such as inheritance.  It also serves as a visual tool for users, 
        /// so they know the list of possible properties from which to select 
        /// when adding options.
        /// </summary>
        /// <param name="category">The category with which to associate the property</param>
        /// <param name="property">The property to associate</param>
        /// <exception cref="Terra.ServerException">Either the property or the category doesn't exist</exception>
        public void AddProperty(Category category, Property property)
        {
            _client.Request("category/property", Method.PUT).
                AddParameter("opco", category.Opco).
                AddParameter("category", category.Slug).
                AddParameter("property", property.Slug).
                MakeRequest();
        }

        /// <summary>
        /// Remove a property from a category.  Note that this does not remove 
        /// any of the relations between options and this node.  It does, 
        /// however, filter the options from any requests that use the properties 
        /// associated with the node as a filter, such as inheritance.
        /// </summary>
        /// <param name="category">A category from which to remove the property</param>
        /// <param name="property">The property to remove</param>
        /// <exception cref="Terra.ServerException">Either the property or the node doesn't exist</exception>
        public void RemoveProperty(Category category, Property property)
        {
            _client.Request("category/property", Method.DELETE).
                AddParameter("opco", category.Opco).
                AddParameter("category", category.Slug).
                AddParameter("property", property.Slug).
                MakeRequest();
        }

        /// <summary>
        /// Find all the options associated with this category.  
        /// <para>
        /// This operation can be a bit slow, as the system has to filter and 
        /// collate options and properties.  It is typically faster to request
        /// a list of properties, then request the options for a selected
        /// property (this is how the Terra UI does things).  Of course by 
        /// "slow", we mean takes around 300ms.
        /// </para>
        /// </summary>
        /// <param name="category">The category of options to retrieve</param>
        /// <returns>A list of Property objects with their Options list filled</returns>
        public List<Property> Options(Category category)
        {
            return _client.Request("category/options").
                AddParameter("opco", category.Opco).
                AddParameter("slug", category.Slug).
                MakeRequest<List<Property>>();
        }

        /// <summary>
        /// Add an option to a category.  The category, option, and property
        /// must all exist in the same operating company.
        /// </summary>
        /// <param name="category">The category to associate the option</param>
        /// <param name="property">The property or "verb" used in the relation</param>
        /// <param name="option">The option being related</param>
        /// <exception cref="Terra.ServerException">Either the option, property or the category doesn't exist</exception>
        public void AddOption(Category category, Property property, Option option)
        {
            _client.Request("category/option", Method.PUT).
                AddParameter("opco", category.Opco).
                AddParameter("category", category.Slug).
                AddParameter("property", property.Slug).
                AddParameter("option", option.Slug).
                MakeRequest();
        }

        /// <summary>
        /// Remove an option from a category.  The category, option, and property
        /// must all exist in the same operating company.
        /// </summary>
        /// <param name="category">The category from which to remove the option</param>
        /// <param name="property">The property or "verb" used in the relation</param>
        /// <param name="option">The option being removed</param>
        /// <exception cref="Terra.ServerException">Either the option, property or the category doesn't exist</exception>
        public void RemoveOption(Category category, Property property, Option option)
        {
            _client.Request("category/option", Method.DELETE).
                AddParameter("opco", category.Opco).
                AddParameter("category", category.Slug).
                AddParameter("property", property.Slug).
                AddParameter("option", option.Slug).
                MakeRequest();
        }

        /// <summary>
        /// Look for the categories that the given category has been mapped
        /// to.  In otherwords, what category-to-category mappings exists for
        /// which this category is the subject of the relation?
        /// </summary>
        /// <param name="category">The subject category of the relations</param>
        /// <returns>A list of object categories</returns>
        /// <exception cref="Terra.ServerException">The subject category does not exist</exception>
        public List<Category> MappedTo(Category category)
        {
            return _client.Request("category/mappings").
                AddParameter("opco", category.Opco).
                AddParameter("slug", category.Slug).
                AddParameter("dir", "to").
                MakeRequest<List<Category>>();
        }

        /// <summary>
        /// Look for the categories mapped to the given category.  In otherwords, 
        /// what category-to-category mappings exists for which this category is 
        /// the object of the relation?
        /// </summary>
        /// <param name="category">The object category of the relations</param>
        /// <returns>A list of subject categories</returns>
        /// <exception cref="Terra.ServerException">The object category does not exist</exception>
        public List<Category> MappedFrom(Category category)
        {
            return _client.Request("category/mappings").
                AddParameter("opco", category.Opco).
                AddParameter("slug", category.Slug).
                AddParameter("dir", "from").
                MakeRequest<List<Category>>();
        }

        /// <summary>
        /// Find the headings mapped to this category.  Like GetMappedFrom, 
        /// what heading-to-category mappings exist for which this category
        /// is the object of the relation?
        /// </summary>
        /// <param name="category">The object category</param>
        /// <returns>The headings mapped to this category</returns>
        /// <exception cref="Terra.ServerException">The object category does not exist</exception>
        public List<Heading> MappedHeadings(Category category)
        {
            return _client.Request("category/headings").
                AddParameter("opco", category.Opco).
                AddParameter("slug", category.Slug).
                AddParameter("dir", "from").
                MakeRequest<List<Heading>>();
        }

        /// <summary>
        /// Map two categories together.  Direction matters:  categories in 
        /// the "from" position will inherit properties and options from the
        /// "to" category.
        /// <para>
        /// Note: a future release of the Terra API will support mapping custom
        /// relation types between categories, in support of new products and
        /// services.  For now, the relation type is "mapped-to".
        /// </para>
        /// </summary>
        /// <param name="from">The subject category, to inherit from the object category</param>
        /// <param name="to">The object category</param>
        /// <exception cref="Terra.ServerException">Either of the categories does not exist or could not be mapped</exception>
        public void MapCategory(Category from, Category to)
        {
            _client.Request("category/mapping", Method.PUT).
                AddParameter("opco", from.Opco).
                AddParameter("from", from.Slug).
                AddParameter("to_opco", to.Opco).
                AddParameter("to", to.Slug).
                MakeRequest();
        }

        /// <summary>
        /// Map a heading to a category.  The heading will inherit the properties
        /// and options from the category.  Note that you may map a heading to 
        /// many different categories, and inherit the sum total of properties 
        /// and options from those categories.
        /// <para>
        /// For reference, the relation between a heading and a category is 
        /// defined as "heading-for".
        /// </para>
        /// </summary>
        /// <param name="from">The heading (subject)</param>
        /// <param name="to">The category (object)</param>
        /// <exception cref="Terra.ServerException">Either the category or heading does not exist or could not be mapped</exception>
        public void MapHeading(Heading from, Category to)
        {
            _client.Request("heading/mapping", Method.PUT).
                AddParameter("opco", from.Opco).
                AddParameter("from", from.Pid).
                AddParameter("to_opco", to.Opco).
                AddParameter("to", to.Slug).
                MakeRequest();
        }

        /// <summary>
        /// Remove the relation between two categories.
        /// <para>
        /// Note: a future release of the Terra API will support mapping custom
        /// relation types between categories, in support of new products and
        /// services.  For now, the relation type is "mapped-to".
        /// </para>
        /// </summary>
        /// <param name="from">The subject category</param>
        /// <param name="to">The object category</param>
        /// <exception cref="Terra.ServerException">Either of the categories does not exist or could not be unmapped</exception>
        public void UnmapCategory(Category from, Category to)
        {
            _client.Request("category/mapping", Method.DELETE).
                AddParameter("opco", from.Opco).
                AddParameter("from", from.Slug).
                AddParameter("to_opco", to.Opco).
                AddParameter("to", to.Slug).
                MakeRequest();
        }

        /// <summary>
        /// Remove the relation between a heading and a category.
        /// </summary>
        /// <param name="from">The heading (subject)</param>
        /// <param name="to">The category (object)</param>
        /// <exception cref="Terra.ServerException">Either the category or heading does not exist or could not be unmapped</exception>
        public void UnmapHeading(Heading from, Category to)
        {
            _client.Request("heading/mapping", Method.DELETE).
                AddParameter("opco", from.Opco).
                AddParameter("from", from.Pid).
                AddParameter("to_opco", to.Opco).
                AddParameter("to", to.Slug).
                MakeRequest();
        }

        /// <summary>
        /// Retrieve the full inheritance of properties and options for this
        /// category, based on the parent-child relations in taxonomies and the
        /// mappings to other categories, based on those categories' own 
        /// inheritance.
        /// </summary>
        /// <param name="category">The category for which to retrieve inheritance</param>
        /// <returns>A list of Property objects, with inherited Options included</returns>
        /// <exception cref="Terra.ServerException">The category does not exist</exception>
        public List<Property> Inheritance(Category category)
        {
            return _client.Request("category/inheritance").
                AddParameter("opco", category.Opco).
                AddParameter("slug", category.Slug).
                MakeRequest<List<Property>>();
        }
    }
}