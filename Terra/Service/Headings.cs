using System;
using System.Collections.Generic;
using RestSharp;

namespace Terra.Service
{
    /// <summary>
    /// Functions around modifying and mapping headings.
    /// <para>
    /// Note that if you're looking for functions around mapping things TO a 
    /// heading, they do not exist.  Terra 2 does not support mapping objects
    /// to headings, only mapping headings to other objects, such as categories.
    /// </para>
    /// <para>
    /// Do not create an instance of this directly.  Instead, call in through 
    /// the Terra.Client.Headings.
    /// </para>
    /// </summary>
    /// <seealso cref="Terra.Client.Headings"/>
    public class Headings
    {
        private Client _client;

        public Headings(Client client)
        {
            _client = client;
        }

        /// <summary>
        /// Get all the headings associated with the given operating company.
        /// This request can take a while, given the large number of headings
        /// associated with each operating company.
        /// </summary>
        /// <param name="opco">The operating company to look towards for headings</param>
        /// <returns>A list of Heading objects</returns>
        /// <exception cref="Terra.ServerException">The given operating company does not exist</exception>
        public List<Heading> All(string opco)
        {
            return _client.Request("headings").
                AddParameter("opco", opco).
                MakeRequest<List<Heading>>();
        }

        /// <summary>
        /// Look for details about a specific heading.  Note that this call 
        /// simply returns the basic information, such as the name and language
        /// of the heading.  It does not return mappings.
        /// <para>
        /// It is useful for confirming the existence of a heading.
        /// </para>
        /// </summary>
        /// <param name="opco">The three or four letter code for the operating company, e.g. "PKT"</param>
        /// <param name="pid">The PID identifier for the heading, unique to the operating company</param>
        /// <returns>A heading</returns>
        /// <exception cref="Terra.ServerException">Either the operating company or heading does not exist</exception>
        public Heading Get(string opco, string pid)
        {
            return _client.Request("heading").
                AddParameter("opco", opco).
                AddParameter("pid", pid).
                MakeRequest<Heading>();
        }

        /// <summary>
        /// Create a new heading for the given operating company.  The heading
        /// name need not be unique, but its PID must be.  
        /// <para>
        /// If the PID is not provided (either null or an empty string), it will
        /// be created by transforming the name of the heading into an SEO-ready
        /// value.  For example, the name "Mexican Restaurants" would be transformed 
        /// into "mexican-restaurants".
        /// </para>
        /// <para>
        /// If language is not supplied it will default to the language of the
        /// operating company.
        /// </para>
        /// </summary>
        /// <param name="opco">The three or four letter code for the operating company</param>
        /// <param name="name">The name of the heading</param>
        /// <param name="pid">A unique identifier for the heading, or null to have one generated</param>
        /// <param name="language">The language of the heading, or null to use the opco's language</param>
        /// <param name="parent">A superheading, to add this new heading directly to a superheading</param>
        /// <returns>The newly created Heading object</returns>
        /// <exception cref="Terra.ServerException">
        /// <list type="bullet">
        /// <item>
        /// <description>If any of the parameters submitted are invalid, returns a status of Not Acceptable.</description>
        /// </item>
        /// <item>
        /// <description>If the PID already exists, returns a status of Conflict.</description>
        /// </item>
        /// <item>
        /// <description>If the operating company or superheading does not exist, returns a status of Not Found.</description>
        /// </item>
        /// </list>
        /// </exception>
        public Heading Create(string opco, string name, string pid = null, string language = null, Superheading parent = null)
        {
            var request = _client.Request("heading", Method.POST).
                AddParameter("opco", opco).
                AddParameter("name", name).
                AddParameter("pid", pid).
                AddParameter("lang", language);

            if (parent != null)
            {
                request.AddParameter("superheading", parent.Slug);
            }

            return request.MakeRequest<Heading>();
        }

        /// <summary>
        /// Update's the name and language of the given heading with the server. 
        /// Will not change the PID or operating company however.  If you modify
        /// either of those, mostly likely you will receive a Not Found status in
        /// the ServerException, indicating the heading object could not be found
        /// on the Terra server.
        /// <para>
        /// Note that this method will return a new Heading object.  The original
        /// object will not be modified.
        /// </para>
        /// </summary>
        /// <param name="heading">An existing heading with its name or language modified</param>
        /// <returns>A new Heading object with the updated information, as confirmation</returns>
        /// <exception cref="Terra.ServerException">
        /// <list type="bullet">
        /// <item>
        /// <description>If any of the parameters submitted are invalid, returns a status of Not Acceptable.</description>
        /// </item>
        /// <item>
        /// <description>If the existing heading could not be found on the server, returns a status of Not Found</description>
        /// </item>
        /// <item>
        /// <description>If the local heading is older than the version on the server, a Precondition Failed status is returned</description>
        /// </item>
        /// </list>
        /// </exception>
        public Heading Update(Heading heading)
        {
            return _client.Request("heading", Method.PUT).
                AddParameter("opco", heading.Opco).
                AddParameter("name", heading.Name).
                AddParameter("pid", heading.Pid).
                AddParameter("lang", heading.Language).
                AddParameter("v", heading.Version).
                MakeRequest<Heading>();
        }

        /// <summary>
        /// Completely deletes the given heading from the Terra server.  
        /// <para>
        /// This method returns nothing.  If the delete is unsuccessful, an
        /// exception will be raised.  Otherwise it completed successfully.
        /// </para>
        /// </summary>
        /// <param name="heading">The heading to delete; only Opco and Pid are used</param>
        /// <exception cref="Terra.ServerException">
        /// Raises a Not Found exception if the heading does not exist, or
        /// a Precondition Failed if the heading on the server is newer than
        /// the one submitted.
        /// </exception>
        public void Delete(Heading heading)
        {
            _client.Request("heading", Method.DELETE).
                AddParameter("opco", heading.Opco).
                AddParameter("pid", heading.Pid).
                AddParameter("v", heading.Version).
                MakeRequest();
        }

        /// <summary>
        /// Get the list of synonyms associated with this heading.
        /// </summary>
        /// <param name="heading">The heading</param>
        /// <returns>A list of synonyms</returns>
        /// <exception cref="Terra.ServerException">The heading does not exist</exception>
        public List<Synonym> Synonyms(Heading heading)
        {
            return _client.Request("heading/synonyms").
                AddParameter("opco", heading.Opco).
                AddParameter("pid", heading.Pid).
                MakeRequest<List<Synonym>>();
        }

        /// <summary>
        /// Create a new synonym and associate it with this heading.
        /// <para>
        /// Synonyms may also be used as a simple translation tool.  When you
        /// create a synonym, by default it is assigned to the same langauge as
        /// the operating company.  However, by assigning a different language
        /// to the synonym, you now have a translation for the heading.
        /// </para>
        /// </summary>
        /// <param name="heading">The heading to associate with the synonym</param>
        /// <param name="name">The human-readable name of the synonym</param>
        /// <param name="slug">An SEO-compliant slug for the synonym; generated if not provided</param>
        /// <param name="language">The language of the name; defaults to the opco's language</param>
        /// <returns>The newly created Synonym</returns>
        /// <exception cref="Terra.ServerException">The heading does not exist, or the synonym already exists</exception>
        public Synonym CreateSynonym(Heading heading, string name, string slug = null, string language = null)
        {
            return _client.Request("heading/synonym", Method.POST).
                AddParameter("opco", heading.Opco).
                AddParameter("name", name).
                AddParameter("slug", slug).
                AddParameter("lang", language).
                AddParameter("heading", heading.Pid).
                MakeRequest<Synonym>();
        }

        /// <summary>
        /// Associate an existing synonym with a heading.
        /// </summary>
        /// <param name="heading">The heading with which to associate the synonym</param>
        /// <param name="synonym">The synonym for the heading</param>
        /// <exception cref="Terra.ServerException">Either the heading or the synonym doesn't exist</exception>
        public void AddSynonym(Heading heading, Synonym synonym)
        {
            _client.Request("heading/synonym", Method.PUT).
                AddParameter("opco", heading.Opco).
                AddParameter("heading", heading.Pid).
                AddParameter("slug", synonym.Slug).
                MakeRequest();
        }

        /// <summary>
        /// This is a convenience method to create or add an existing synonym
        /// (or translation) to a heading.  If the synonym does not already
        /// exist, it is created with a default slug (and default language, if
        /// not otherwise indicated).  The synonym, existing or new, is
        /// associated with the heading.
        /// </summary>
        /// <param name="heading">The heading to associate with this synonym</param>
        /// <param name="synonym">The new or existing name of a synonym</param>
        /// <param name="language">The language of the synonym; defaults to the opco's language</param>
        /// <returns>The new or existing synonym</returns>
        /// <exception cref="Terra.ServerException">The heading does not exist</exception>
        public Synonym AddSynonym(Heading heading, string synonym, string language = null)
        {
            var slug = _client.Slugify(synonym);
            try
            {
                var existing = _client.Synonyms.Get(heading.Opco, slug);
                _client.Request("heading/synonym", Method.PUT).
                    AddParameter("opco", heading.Opco).
                    AddParameter("heading", heading.Pid).
                    AddParameter("slug", slug).
                    MakeRequest();
                return existing;
            }
            catch (ServerException e)
            {
                if (e.Status == System.Net.HttpStatusCode.NotFound)
                {
                    return CreateSynonym(heading, synonym, slug, language);
                }
                else
                {
                    throw e;
                }
            }
        }

        /// <summary>
        /// Remove the synonym with from the heading.  Note that this doesn't
        /// delete the synonym, simply removes its association from this 
        /// heading.
        /// </summary>
        /// <param name="heading">The heading with which to disassociate the synonym</param>
        /// <param name="synonym">The synonym for the heading</param>
        /// <exception cref="Terra.ServerException">Either the heading or the synonym doesn't exist</exception>
        public void RemoveSynonym(Heading heading, Synonym synonym)
        {
            _client.Request("heading/synonym", Method.DELETE).
                AddParameter("opco", heading.Opco).
                AddParameter("heading", heading.Pid).
                AddParameter("slug", synonym.Slug).
                MakeRequest();
        }

        /// <summary>
        /// Get the superheadings to which this heading belongs.
        /// </summary>
        /// <param name="heading">The child heading</param>
        /// <returns>A list of Superheadings</returns>
        public List<Superheading> Parents(Heading heading)
        {
            return _client.Request("heading/parents").
                AddParameter("opco", heading.Opco).
                AddParameter("pid", heading.Pid).
                MakeRequest<List<Superheading>>();
        }

        /// <summary>
        /// Look for the categories to which the given heading has been mapped.
        /// In otherwords, what heading-to-category mappings exists for which 
        /// this heading is the subject of the relation?
        /// </summary>
        /// <param name="heading">The subject heading of the relations</param>
        /// <returns>A list of object categories</returns>
        /// <exception cref="Terra.ServerException">The subject heading does not exist</exception>
        public List<Category> MappedTo(Heading heading)
        {
            return _client.Request("heading/mappings").
                AddParameter("opco", heading.Opco).
                AddParameter("pid", heading.Pid).
                MakeRequest<List<Category>>();
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
        /// <para>
        /// This is just a convenience method that does the exact same thing as
        /// Terra.Client.Categories.MapHeading.
        /// </para>
        /// </summary>
        /// <seealso cref="Terra.Service.Categories.MapHeading"/>
        /// <param name="from">The heading (subject)</param>
        /// <param name="to">The category (object)</param>
        /// <exception cref="Terra.ServerException">Either the heading or the category does not exist or could not be mapped</exception>
        public void MapHeading(Heading from, Category to)
        {
            _client.Categories.MapHeading(from, to);
        }

        /// <summary>
        /// Remove the relation between a heading and a category.
        /// <para>
        /// This is just a convenience method that does the exact same thing as
        /// Terra.Client.Categories.UnapHeading.
        /// </para>
        /// </summary>
        /// <seealso cref="Terra.Service.Categories.UnmapHeading"/>
        /// <param name="from">The heading (subject)</param>
        /// <param name="to">The category (object)</param>
        /// <exception cref="Terra.ServerException">Either the category or heading does not exist or could not be unmapped</exception>
        public void UnmapHeading(Heading from, Category to)
        {
            _client.Categories.UnmapHeading(from, to);
        }

        /// <summary>
        /// Retrieve the full inheritance of properties and options for this
        /// heading, based on the mappings to other categories and those 
        /// categories' own inheritance.
        /// </summary>
        /// <param name="heading">The heading for which to retrieve inheritance</param>
        /// <returns>A list of Property objects, with inherited Options included</returns>
        /// <exception cref="Terra.ServerException">The heading does not exist</exception>
        public List<Property> Inheritance(Heading heading)
        {
            return _client.Request("heading/inheritance").
                AddParameter("opco", heading.Opco).
                AddParameter("pid", heading.Pid).
                MakeRequest<List<Property>>();
        }
    }
}