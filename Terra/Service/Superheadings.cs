using System;
using System.Collections.Generic;
using RestSharp;

namespace Terra.Service
{
    /// <summary>
    /// Superheadings provide for a very limited grouping of headings within
    /// Terra, to reduce the amount of information that has to come back when
    /// working with headings.  
    /// <para>
    /// Do not create an instance of this directly.  Instead, call in through 
    /// the Terra.Client.Superheadings.
    /// </para>
    /// </summary>
    /// <seealso cref="Terra.Client.Superheadings"/>
    public class Superheadings
    {
        private Client _client;

        public Superheadings(Client client)
        {
            _client = client;
        }

        /// <summary>
        /// Returns all the superheadings associated with the given opco.
        /// </summary>
        /// <param name="opco">The three or four letter code for the operating company</param>
        /// <returns>A list of Superheadings</returns>
        /// <exception cref="Terra.ServerException">The operating company does not exist</exception>
        public List<Superheading> All(string opco)
        {
            return _client.Request("superheadings").AddParameter("opco", opco).MakeRequest<List<Superheading>>();
        }

        /// <summary>
        /// Look up a superheading by its slug.  Useful for checking to see if an
        /// superheading exists.
        /// </summary>
        /// <param name="opco">The three or four letter code for the opco</param>
        /// <param name="slug">The superheadings's slug</param>
        /// <returns>An Superheading object</returns>
        /// <exception cref="Terra.ServerException">The operating company or superheading does not exist</exception>
        public Superheading Get(string opco, string slug)
        {
            return _client.Request("superheading").AddParameter("opco", opco).AddParameter("slug", slug).MakeRequest<Superheading>();
        }

        /// <summary>
        /// Create a new superheading.  
        /// </summary>
        /// <param name="opco">The three or four letter code for the operating company</param>
        /// <param name="name">The human-readable name for this superheading</param>
        /// <param name="slug">A unique slug for this superheading; superheadingal, will be generated from the name if not provided</param>
        /// <param name="external">An third-party external identifier for this superheading (superheadingal)</param>
        /// <param name="language">The two-letter ISO language for the superheading's name; defaults to the opco's language</param>
        /// <returns>The newly created Superheading</returns>
        /// <exception cref="Terra.ServerException">
        /// <list type="bullet">
        /// <item>
        /// <description>If any of the parameters submitted are invalid, returns a status of Not Acceptable.</description>
        /// </item>
        /// <item>
        /// <description>If the slug already exists, returns a status of Conflict.</description>
        /// </item>
        /// <item>
        /// <description>If the operating company, relatedTo, or relatedBy does not exist, returns a status of Not Found.</description>
        /// </item>
        /// </list>
        /// </exception>
        public Superheading Create(string opco, string name, string slug = null, string external = null, string language = null)
        {
            return _client.Request("superheading", Method.POST).
                AddParameter("opco", opco).
                AddParameter("name", name).
                AddParameter("slug", slug).
                AddParameter("external", external).
                AddParameter("lang", language).
                MakeRequest<Superheading>();
        }

        /// <summary>
        /// Update the name, external identifier, or language for this superheading.  
        /// Neither the operating company nor the slug may be modified.
        /// </summary>
        /// <param name="superheading">An superheading with the updated information</param>
        /// <returns>A new Superheading object, with the updates in place</returns>
        /// <exception cref="Terra.ServerException">
        /// <list type="bullet">
        /// <item>
        /// <description>If any of the parameters submitted are invalid, returns a status of Not Acceptable.</description>
        /// </item>
        /// <item>
        /// <description>If the existing superheading could not be found on the server, returns a status of Not Found</description>
        /// </item>
        /// <item>
        /// <description>If the local superheading is older than the version on the server, a Precondition Failed status is returned</description>
        /// </item>
        /// </list>
        /// </exception>
        public Superheading Update(Superheading superheading)
        {
            return _client.Request("superheading", Method.PUT).
                AddParameter("opco", superheading.Opco).
                AddParameter("name", superheading.Name).
                AddParameter("slug", superheading.Slug).
                AddParameter("external", superheading.External).
                AddParameter("lang", superheading.Language).
                MakeRequest<Superheading>();
        }

        /// <summary>
        /// Completely delete a superheading from Terra.  If you delete a 
        /// superheading, the headings will still be available from the
        /// operating company itself.
        /// </summary>
        /// <param name="superheading">The superheading to delete</param>
        /// <exception cref="Terra.ServerException">The superheading does not exist</exception>
        public void Delete(Superheading superheading)
        {
            _client.Request("superheading", Method.DELETE).
                AddParameter("opco", superheading.Opco).
                AddParameter("slug", superheading.Slug).
                MakeRequest();
        }

        /// <summary>
        /// Get the list of synonyms associated with this superheading.
        /// </summary>
        /// <param name="superheading">The superheading</param>
        /// <returns>A list of synonyms</returns>
        /// <exception cref="Terra.ServerException">The superheading does not exist</exception>
        public List<Synonym> Synonyms(Superheading superheading)
        {
            return _client.Request("superheading/synonyms").
                AddParameter("opco", superheading.Opco).
                AddParameter("slug", superheading.Slug).
                MakeRequest<List<Synonym>>();
        }

        /// <summary>
        /// Create a new synonym and associate it with this superheading.
        /// <para>
        /// Synonyms may also be used as a simple translation tool.  When you
        /// create a synonym, by default it is assigned to the same langauge as
        /// the operating company.  However, by assigning a different language
        /// to the synonym, you now have a translation for the superheading.
        /// </para>
        /// </summary>
        /// <param name="superheading">The superheading to associate with the synonym</param>
        /// <param name="name">The human-readable name of the synonym</param>
        /// <param name="slug">An SEO-compliant slug for the synonym; generated if not provided</param>
        /// <param name="language">The language of the name; defaults to the opco's language</param>
        /// <returns>The newly created Synonym</returns>
        /// <exception cref="Terra.ServerException">The superheading does not exist, or the synonym already exists</exception>
        public Synonym CreateSynonym(Superheading superheading, string name, string slug = null, string language = null)
        {
            return _client.Request("superheading/synonym", Method.POST).
                AddParameter("opco", superheading.Opco).
                AddParameter("name", name).
                AddParameter("slug", slug).
                AddParameter("lang", language).
                AddParameter("superheading", superheading.Slug).
                MakeRequest<Synonym>();
        }

        /// <summary>
        /// Associate an existing synonym with a superheading.
        /// </summary>
        /// <param name="superheading">The superheading with which to associate the synonym</param>
        /// <param name="synonym">The synonym for the superheading</param>
        /// <exception cref="Terra.ServerException">Either the superheading or the synonym doesn't exist</exception>
        public void AddSynonym(Superheading superheading, Synonym synonym)
        {
            _client.Request("superheading/synonym", Method.PUT).
                AddParameter("opco", superheading.Opco).
                AddParameter("superheading", superheading.Slug).
                AddParameter("slug", synonym.Slug).
                MakeRequest();
        }

        /// <summary>
        /// This is a convenience method to create or add an existing synonym
        /// (or translation) to a superheading.  If the synonym does not already
        /// exist, it is created with a default slug (and default language, if
        /// not otherwise indicated).  The synonym, existing or new, is
        /// associated with the superheading.
        /// </summary>
        /// <param name="superheading">The superheading to associate with this synonym</param>
        /// <param name="synonym">The new or existing name of a synonym</param>
        /// <param name="language">The language of the synonym; defaults to the opco's language</param>
        /// <returns>The new or existing synonym</returns>
        /// <exception cref="Terra.ServerException">The superheading does not exist</exception>
        public Synonym AddSynonym(Superheading superheading, string synonym, string language = null)
        {
            var slug = _client.Slugify(synonym);
            try
            {
                var existing = _client.Synonyms.Get(superheading.Opco, slug);
                _client.Request("superheading/synonym", Method.PUT).
                    AddParameter("opco", superheading.Opco).
                    AddParameter("superheading", superheading.Slug).
                    AddParameter("slug", slug).
                    MakeRequest();
                return existing;
            }
            catch (ServerException e)
            {
                if (e.Status == System.Net.HttpStatusCode.NotFound)
                {
                    return CreateSynonym(superheading, synonym, slug, language);
                }
                else
                {
                    throw e;
                }
            }
        }

        /// <summary>
        /// Remove the synonym with from the superheading.  Note that this doesn't
        /// delete the synonym, simply removes its association from this 
        /// superheading.
        /// </summary>
        /// <param name="superheading">The superheading with which to disassociate the synonym</param>
        /// <param name="synonym">The synonym for the superheading</param>
        /// <exception cref="Terra.ServerException">Either the superheading or the synonym doesn't exist</exception>
        public void RemoveSynonym(Superheading superheading, Synonym synonym)
        {
            _client.Request("superheading/synonym", Method.DELETE).
                AddParameter("opco", superheading.Opco).
                AddParameter("superheading", superheading.Slug).
                AddParameter("slug", synonym.Slug).
                MakeRequest();
        }

        /// <summary>
        /// Retrieve the headings associated with this superheading.
        /// </summary>
        /// <param name="superheading">The superheading parent</param>
        /// <returns>A list of headings</returns>
        public List<Heading> Headings(Superheading superheading)
        {
            return null;
        }

        /// <summary>
        /// Add a heading to the given superheading.  You may repeatedly add
        /// a heading to a superheading with no ill or adverse effects, and
        /// no exceptions will be thrown.  
        /// </summary>
        /// <param name="superheading">The parent superheading</param>
        /// <param name="heading">The child heading</param>
        /// <exception cref="Terra.ServerException">Either the superheading or the heading doesn't exist</exception>
        public void AddHeading(Superheading superheading, Heading heading)
        {

        }

        /// <summary>
        /// Remove the heading from the superheading.  Removing a heading from
        /// a superheading that doesn't have that heading has no effect and
        /// generates no exceptions.
        /// </summary>
        /// <param name="superheading">The superheading parent</param>
        /// <param name="heading">The child heading</param>
        /// <exception cref="Terra.ServerException">Either the superheading or the heading doesn't exist</exception>
        public void RemoveHeading(Superheading superheading, Heading heading)
        {
        }
    }
}
