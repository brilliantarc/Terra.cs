using System;
using System.Collections.Generic;
using RestSharp;

namespace Terra.Service
{
    /// <summary>
    /// Modify and interact with options on a category or taxonomy.  Much of the
    /// functionality for mapping options to categories and taxonomies resides
    /// in those services, e.g. Terra.Service.Categories.AddOption.  
    /// Functionality here focuses on creating, updating, and deleting options.
    /// <para>
    /// Do not create an instance of this directly.  Instead, call in through 
    /// the Terra.Client.Options.
    /// </para>
    /// </summary>
    /// <seealso cref="Terra.Client.Options"/>
    /// <seealso cref="Terra.Service.Categories"/>
    /// <seealso cref="Terra.Service.Taxonomies"/>
    public class Options
    {
        private Client _client;

        public Options(Client client)
        {
            _client = client;
        }


        /// <summary>
        /// Get all the options defined for the operating company.  This may return
        /// options that are not in use, i.e. not associated with any taxonomies or
        /// categories, but are still defined in the system.
        /// </summary>
        /// <param name="opco">The three or four letter code for the operating company</param>
        /// <returns>A list of all Option objects associate with the operating company</returns>
        public List<Option> All(String opco)
        {
            return _client.Request("options").
                AddParameter("opco", opco).
                MakeRequest<List<Option>>();
        }

        /// <summary>
        /// Look up an option by its slug.  Useful for checking to see if an
        /// option exists.
        /// </summary>
        /// <param name="opco">The three or four letter code for the opco</param>
        /// <param name="slug">The options's slug</param>
        /// <returns>An Option object</returns>
        /// <exception cref="Terra.ServerException">The operating company or option does not exist</exception>
        public Option Get(string opco, string slug)
        {
            return _client.Request("option").AddParameter("opco", opco).AddParameter("slug", slug).MakeRequest<Option>();
        }

        /// <summary>
        /// Create a new option.  
        /// </summary>
        /// <param name="opco">The three or four letter code for the operating company</param>
        /// <param name="name">The human-readable name for this option</param>
        /// <param name="slug">A unique slug for this option; optional, will be generated from the name if not provided</param>
        /// <param name="external">An third-party external identifier for this option (optional)</param>
        /// <param name="language">The two-letter ISO language for the option's name; defaults to the opco's language</param>
        /// <param name="relatedTo">The category or taxonomy using this option (optional)</param>
        /// <param name="relatedBy">The property by which this option is related to the relatedTo value</param>
        /// <returns>The newly created Option</returns>
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
        public Option Create(string opco, string name, string slug = null, string external = null, string language = null,
            Node relatedTo = null, Property relatedBy = null)
        {
            var request = _client.Request("option", Method.POST).
                AddParameter("opco", opco).
                AddParameter("name", name).
                AddParameter("slug", slug).
                AddParameter("external", external).
                AddParameter("lang", language);

            if (relatedBy != null)
            {
                if (relatedTo is Taxonomy)
                {
                    request.AddParameter("taxonomy", ((Taxonomy)relatedTo).Slug);
                }
                else if (relatedTo is Category)
                {
                    request.AddParameter("category", ((Category)relatedTo).Slug);
                }

                request.AddParameter("property", relatedBy.Slug);
            }

            return request.MakeRequest<Option>();
        }

        /// <summary>
        /// Update the name, external identifier, or language for this option.  
        /// Neither the operating company nor the slug may be modified.
        /// </summary>
        /// <param name="option">An option with the updated information</param>
        /// <returns>A new Option object, with the updates in place</returns>
        /// <exception cref="Terra.ServerException">
        /// <list type="bullet">
        /// <item>
        /// <description>If any of the parameters submitted are invalid, returns a status of Not Acceptable.</description>
        /// </item>
        /// <item>
        /// <description>If the existing option could not be found on the server, returns a status of Not Found</description>
        /// </item>
        /// <item>
        /// <description>If the local option is older than the version on the server, a Precondition Failed status is returned</description>
        /// </item>
        /// </list>
        /// </exception>
        public Option Update(Option option)
        {
            return _client.Request("option", Method.PUT).
                AddParameter("opco", option.Opco).
                AddParameter("name", option.Name).
                AddParameter("slug", option.Slug).
                AddParameter("external", option.External).
                AddParameter("lang", option.Language).
                AddParameter("v", option.Version).
                MakeRequest<Option>();
        }

        /// <summary>
        /// Completely delete an option from Terra.  This will remove the option 
        /// from every category and taxonomy in the operating company.
        /// </summary>
        /// <param name="option">The option to delete</param>
        /// <exception cref="Terra.ServerException">The option does not exist</exception>
        public void Delete(Option option)
        {
            _client.Request("option", Method.DELETE).
                AddParameter("opco", option.Opco).
                AddParameter("slug", option.Slug).
                AddParameter("v", option.Version).
                MakeRequest();
        }

        /// <summary>
        /// Get the list of synonyms associated with this option.
        /// </summary>
        /// <param name="option">The option</param>
        /// <returns>A list of synonyms</returns>
        /// <exception cref="Terra.ServerException">The option does not exist</exception>
        public List<Synonym> Synonyms(Option option)
        {
            return _client.Request("option/synonyms").
                AddParameter("opco", option.Opco).
                AddParameter("slug", option.Slug).
                MakeRequest<List<Synonym>>();
        }

        /// <summary>
        /// Create a new synonym and associate it with this option.
        /// <para>
        /// Synonyms may also be used as a simple translation tool.  When you
        /// create a synonym, by default it is assigned to the same langauge as
        /// the operating company.  However, by assigning a different language
        /// to the synonym, you now have a translation for the option.
        /// </para>
        /// </summary>
        /// <param name="option">The option to associate with the synonym</param>
        /// <param name="name">The human-readable name of the synonym</param>
        /// <param name="slug">An SEO-compliant slug for the synonym; generated if not provided</param>
        /// <param name="language">The language of the name; defaults to the opco's language</param>
        /// <returns>The newly created Synonym</returns>
        /// <exception cref="Terra.ServerException">The option does not exist, or the synonym already exists</exception>
        public Synonym CreateSynonym(Option option, string name, string slug = null, string language = null)
        {
            return _client.Request("option/synonym", Method.POST).
                AddParameter("opco", option.Opco).
                AddParameter("name", name).
                AddParameter("slug", slug).
                AddParameter("lang", language).
                AddParameter("option", option.Slug).
                MakeRequest<Synonym>();
        }

        /// <summary>
        /// Associate an existing synonym with a option.
        /// </summary>
        /// <param name="option">The option with which to associate the synonym</param>
        /// <param name="synonym">The synonym for the option</param>
        /// <exception cref="Terra.ServerException">Either the option or the synonym doesn't exist</exception>
        public void AddSynonym(Option option, Synonym synonym)
        {
            _client.Request("option/synonym", Method.PUT).
                AddParameter("opco", option.Opco).
                AddParameter("option", option.Slug).
                AddParameter("slug", synonym.Slug).
                MakeRequest();
        }

        /// <summary>
        /// This is a convenience method to create or add an existing synonym
        /// (or translation) to a option.  If the synonym does not already
        /// exist, it is created with a default slug (and default language, if
        /// not otherwise indicated).  The synonym, existing or new, is
        /// associated with the option.
        /// </summary>
        /// <param name="option">The option to associate with this synonym</param>
        /// <param name="synonym">The new or existing name of a synonym</param>
        /// <param name="language">The language of the synonym; defaults to the opco's language</param>
        /// <returns>The new or existing synonym</returns>
        /// <exception cref="Terra.ServerException">The option does not exist</exception>
        public Synonym AddSynonym(Option option, string synonym, string language = null)
        {
            var slug = _client.Slugify(synonym);
            try
            {
                var existing = _client.Synonyms.Get(option.Opco, slug);
                _client.Request("option/synonym", Method.PUT).
                    AddParameter("opco", option.Opco).
                    AddParameter("option", option.Slug).
                    AddParameter("slug", slug).
                    MakeRequest();
                return existing;
            }
            catch (ServerException e)
            {
                if (e.Status == System.Net.HttpStatusCode.NotFound)
                {
                    return CreateSynonym(option, synonym, slug, language);
                }
                else
                {
                    throw e;
                }
            }
        }

        /// <summary>
        /// Remove the synonym with from the option.  Note that this doesn't
        /// delete the synonym, simply removes its association from this 
        /// option.
        /// </summary>
        /// <param name="option">The option with which to disassociate the synonym</param>
        /// <param name="synonym">The synonym for the option</param>
        /// <exception cref="Terra.ServerException">Either the option or the synonym doesn't exist</exception>
        public void RemoveSynonym(Option option, Synonym synonym)
        {
            _client.Request("option/synonym", Method.DELETE).
                AddParameter("opco", option.Opco).
                AddParameter("option", option.Slug).
                AddParameter("slug", synonym.Slug).
                MakeRequest();
        }

        /// <summary>
        /// Get the "sub" options for this option.  Options can be related to other
        /// options, as sort of "child" options.  This functionality isn't widely
        /// used, and if you're considering using suboptions, you should first
        /// consider using synonyms instead.
        /// </summary>
        /// <param name="option">The parent option</param>
        /// <returns>The options related to the given parent option</returns>
        /// <exception cref="Terra.ServerException">The option doesn't exist</exception>
        public List<Option> Suboptions(Option option)
        {
            return _client.Request("option/sub").
                AddParameter("opco", option.Opco).
                AddParameter("slug", option.Slug).
                MakeRequest<List<Option>>();
        }

        /// <summary>
        /// Associate an option as a "suboption" of a parent option.  This
        /// functionality isn't widely used, and if you're considering using
        /// suboptions, you should first consider using synonyms instead.
        /// </summary>
        /// <param name="option">The parent option</param>
        /// <param name="suboption">The option to relate to the parent</param>
        /// <exception cref="Terra.ServerException">Either the option or the suboption doesn't exist</exception>
        public void AddSuboption(Option option, Option suboption)
        {
            _client.Request("option/sub", Method.PUT).
                AddParameter("opco", option.Opco).
                AddParameter("option", option.Slug).
                AddParameter("suboption", suboption.Slug).
                MakeRequest();
        }

        /// <summary>
        /// Remove the association between an option and its "suboption".  This
        /// functionality isn't widely used, and if you're considering using
        /// suboptions, you should first consider using synonyms instead.
        /// </summary>
        /// <param name="option">The parent option</param>
        /// <param name="suboption">The option to remove (does not delete the suboption)</param>
        /// <exception cref="Terra.ServerException">Either the option or the suboption doesn't exist</exception>
        public void RemoveSuboption(Option option, Option suboption)
        {
            _client.Request("option/sub", Method.DELETE).
                AddParameter("opco", option.Opco).
                AddParameter("option", option.Slug).
                AddParameter("suboption", suboption.Slug).
                MakeRequest();
        }
    }
}
