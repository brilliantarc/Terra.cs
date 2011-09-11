﻿using System;
using System.Collections.Generic;
using RestSharp;

namespace Terra.Service
{
    /// <summary>
    /// Some basic functionality in support of synonyms.  Similar to options,
    /// most of the true functionality useful surrounding synonyms resides with
    /// the other services, such as Terra.Service.Categories.  This service
    /// is primarily for updating and mass-deleting synonyms.
    /// <para>
    /// Do not create an instance of this directly.  Instead, call in through 
    /// the Terra.Client.Synonyms.
    /// </para>
    /// </summary>
    /// <seealso cref="Terra.Client.Synonyms"/>
    public class Synonyms
    {
        private Client _client;

        public Synonyms(Client client)
        {
            _client = client;
        }

        /// <summary>
        /// Look up an synonym by its slug.  Useful for checking to see if an
        /// synonym exists.
        /// </summary>
        /// <param name="opco">The three or four letter code for the opco</param>
        /// <param name="slug">The synonyms's slug</param>
        /// <returns>An Synonym object</returns>
        /// <exception cref="Terra.ServerException">The operating company or synonym does not exist</exception>
        public Synonym Get(string opco, string slug)
        {
            return _client.Request("synonym").AddParameter("opco", opco).AddParameter("slug", slug).MakeRequest<Synonym>();
        }

        /// <summary>
        /// Create a new synonym.  
        /// <para>
        /// This will create a "free" synonym in the operating company's 
        /// portfolio; the majority of the time this is not what you want.
        /// Instead, see the individual category, taxonomy, etc. services for
        /// methods to create synonyms and attach them to another meme in the
        /// same call.
        /// </para>
        /// </summary>
        /// <seealso cref="Terra.Service.Categories.CreateSynonym"/>
        /// <seealso cref="Terra.Service.Taxonomies.CreateSynonym"/>
        /// <seealso cref="Terra.Service.Properties.CreateSynonym"/>
        /// <seealso cref="Terra.Service.Options.CreateSynonym"/>
        /// <seealso cref="Terra.Service.Headings.CreateSynonym"/>
        /// <seealso cref="Terra.Service.Superheadings.CreateSynonym"/>
        /// <param name="opco">The three or four letter code for the operating company</param>
        /// <param name="name">The human-readable name for this synonym</param>
        /// <param name="slug">A unique slug for this synonym; synonymal, will be generated from the name if not provided</param>
        /// <param name="external">An third-party external identifier for this synonym (synonymal)</param>
        /// <param name="language">The two-letter ISO language for the synonym's name; defaults to the opco's language</param>
        /// <returns>The newly created Synonym</returns>
        /// <exception cref="Terra.ServerException">
        /// <list type="bullet">
        /// <item>
        /// <description>If any of the parameters submitted are invalid, returns a status of Not Acceptable.</description>
        /// </item>
        /// <item>
        /// <description>If the slug already exists, returns a status of Conflict.</description>
        /// </item>
        /// </list>
        /// </exception>
        public Synonym Create(string opco, string name, string slug = null, string external = null, string language = null)
        {
            return _client.Request("synonym", Method.POST).
                AddParameter("opco", opco).
                AddParameter("name", name).
                AddParameter("slug", slug).
                AddParameter("external", external).
                AddParameter("lang", language).
                MakeRequest<Synonym>();
        }

        /// <summary>
        /// Update the name, external identifier, or language for this synonym.  
        /// Neither the operating company nor the slug may be modified.
        /// </summary>
        /// <param name="synonym">An synonym with the updated information</param>
        /// <returns>A new Synonym object, with the updates in place</returns>
        /// <exception cref="Terra.ServerException">
        /// <list type="bullet">
        /// <item>
        /// <description>If any of the parameters submitted are invalid, returns a status of Not Acceptable.</description>
        /// </item>
        /// <item>
        /// <description>If the existing synonym could not be found on the server, returns a status of Not Found</description>
        /// </item>
        /// <item>
        /// <description>If the local synonym is older than the version on the server, a Precondition Failed status is returned</description>
        /// </item>
        /// </list>
        /// </exception>
        public Synonym Update(Synonym synonym)
        {
            return _client.Request("synonym", Method.PUT).
                AddParameter("opco", synonym.Opco).
                AddParameter("name", synonym.Name).
                AddParameter("slug", synonym.Slug).
                AddParameter("external", synonym.External).
                AddParameter("lang", synonym.Language).
                MakeRequest<Synonym>();
        }

        /// <summary>
        /// Completely delete an synonym from Terra.  This will remove the synonym 
        /// from every meme it is related to in the operating company.  In most
        /// cases, you'll likely want something along the lines of 
        /// Terra.Service.Categories.RemoveSynonym instead.
        /// </summary>
        /// <seealso cref="Terra.Service.Categories.RemoveSynonym"/>
        /// <seealso cref="Terra.Service.Taxonomies.RemoveSynonym"/>
        /// <seealso cref="Terra.Service.Properties.RemoveSynonym"/>
        /// <seealso cref="Terra.Service.Options.RemoveSynonym"/>
        /// <seealso cref="Terra.Service.Headings.RemoveSynonym"/>
        /// <seealso cref="Terra.Service.Superheadings.RemoveSynonym"/>
        /// <param name="synonym">The synonym to delete</param>
        /// <exception cref="Terra.ServerException">The synonym does not exist</exception>
        public void Delete(Synonym synonym)
        {
            _client.Request("synonym", Method.DELETE).
                AddParameter("opco", synonym.Opco).
                AddParameter("slug", synonym.Slug).
                MakeRequest();
        }
    }
}