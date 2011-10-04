using System;
using System.Collections.Generic;
using RestSharp;
using Newtonsoft.Json;

namespace Terra
{
    /// <summary>
    /// Every request to the Terra server should come through the client class 
    /// or one of its service offspring.
    /// <para>
    /// You should use a single instance of Terra.Client for each user, as 
    /// the user's credentials for accessing the API are stored within the
    /// client upon successful authentication.  The Terra.Client is 
    /// thread-safe, so you may reuse it as such, but only for the same
    /// user account.  We recommend authenticating on a single thread, so
    /// as not to unintentionally overwrite the credentials of an 
    /// authenticated user.  The next time you authenticate the same instance 
    /// of Terra.Client, it will replace the credentials with those of the
    /// newly authenticated user.  This could obviously cause security and
    /// tracking issues.
    /// </para>
    /// </summary>
    public class Client
    {
        /// <summary>
        /// The currently authenticated user account
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// A reference to service calls relating to operating companies.
        /// </summary>
        public Service.OperatingCompanies OperatingCompanies
        {
            get { return new Service.OperatingCompanies(this); }
            protected set { }
        }

        /// <summary>
        /// A reference to service calls relating to taxonomies.
        /// </summary>
        public Service.Taxonomies Taxonomies
        {
            get { return new Service.Taxonomies(this); }
            protected set { }
        }

        /// <summary>
        /// A reference to service calls relating to categories.
        /// </summary>
        public Service.Categories Categories
        {
            get { return new Service.Categories(this); }
            protected set { }
        }

        /// <summary>
        /// A reference to service calls relating to properties.
        /// </summary>
        public Service.Properties Properties
        {
            get { return new Service.Properties(this); }
            protected set { }
        }

        /// <summary>
        /// A reference to service calls relating to options.
        /// </summary>
        public Service.Options Options
        {
            get { return new Service.Options(this); }
            protected set { }
        }

        /// <summary>
        /// A reference to service calls relating to synonyms (most relevant 
        /// calls relating to synonyms are on the various Taxonomies, Categories,
        /// Options, and other similar services).
        /// </summary>
        public Service.Synonyms Synonyms
        {
            get { return new Service.Synonyms(this); }
            protected set { }
        }

        /// <summary>
        /// A reference to service calls relating to headings.
        /// </summary>
        public Service.Headings Headings
        {
            get { return new Service.Headings(this); }
            protected set { }
        }

        /// <summary>
        /// A reference to service calls relating to superheadings.
        /// </summary>
        public Service.Superheadings Superheadings
        {
            get { return new Service.Superheadings(this); }
            protected set { }
        }

        /// <summary>
        /// Services for managing user accounts, forgotten passwords, messages,
        /// etc.
        /// </summary>
        public Service.Users Users
        {
            get { return new Service.Users(this); }
            protected set { }
        }

        private RestClient _client;

        /// <summary>
        /// Reference to the original RestSharp RestClient object
        /// </summary>
        public RestClient Base
        {
            get { return _client; }
            protected set { }
        }

        /// <summary>
        /// Create a new connection to the Terra REST server.  Each user should have
        /// his or her own connection to the server.  While the Client class is 
        /// thread-safe, do not share instances between users.
        /// </summary>
        /// <param name="url">The base URL to the REST server, e.g. http://terra.brilliantarc.com/api</param>
        public Client(string url)
        {
            _client = new RestClient();
            _client.BaseUrl = url;
        }

        /// <summary>
        /// Build and send a RESTful JSON request to the Terra API server, and then
        /// parse ther results.
        /// <para>
        /// This is used internally by the various service classes to call the Terra
        /// API server.  It is a low-level call.  You are welcome to reuse it to call
        /// services directly, should the need arise, but for general applications
        /// you shouldn't have to.
        /// </para>
        /// </summary>
        /// <param name="resource">The path to the resource on the Terra server, e.g. "properties", or "category/properties"</param>
        /// <param name="method">Sends the request as this HTTP method; defaults to Method.GET</param>
        /// <returns>The request object, which you should then make AddParameter calls with and finally a MakeRequest, GenericRequest, or StringRequest</returns>
        public Request Request(string resource, Method method = Method.GET)
        {
            return new Request(this, resource, method);
        }

        /// <summary>
        /// Authenticate a user's account information for this connection.  Again, 
        /// there should only be a single user account per connection.  If you 
        /// authenticate another user account, it will replace the previous account
        /// information in this instance.
        /// </summary>
        /// <param name="login">The user's account login</param>
        /// <param name="password">The user's account password</param>
        /// <returns>An instance of the Client object, so you can chain calls</returns>
        public Client Authenticate(string login, string password)
        {
            User = Request("user_session", Method.POST).
                AddParameter("login", login).
                AddParameter("password", password).
                MakeRequest<User>();
            return this;
        }

        /// <summary>
        /// Perform a free-text search for categories, properties, options, etc. 
        /// in Terra.  Will return a SearchResult object which contains not only
        /// the matches for the search, but information about refinements and
        /// the total number of results possible.
        /// <para>
        /// Searches are localized to the various languages, in order to apply
        /// language-specific stemming and other operations.  While you may 
        /// search across operating companies, you may not search across 
        /// languages.  Unfortunately this is a limitation of the search engine
        /// used in Terra.
        /// </para>
        /// </summary>
        /// <param name="language">The language in which to conduct the search</param>
        /// <param name="terms">A set of keywords or phrases to search for</param>
        /// <param name="definitions">Limit the search to memes of this type, e.g. ["Categories", "Taxonomies"]</param>
        /// <param name="opco">Limit the search to a specific opco</param>
        /// <param name="from">Start from this result number (defaults to the beginning, 0; for pagination)</param>
        /// <param name="max">Limit the number of results to this number, at most (defaults to 10; for pagination)</param>
        /// <returns>A collection of memes (categories, taxonomies, options), in order of search relevance</returns>
        public List<Node> Search(string language, string terms, List<String> definitions = null, string opco = null, int from = 0, int max = 10)
        {
            var request = Request("search").
                AddParameter("lang", language).
                AddParameter("q", terms).
                AddParameter("from", from).
                AddParameter("max", max);

            if (definitions != null)
            {
                request.AddParameter("definitions", String.Join(",", definitions));
            }

            return request.GenericRequest();
        }

        /// <summary>
        /// Run a traversal function on the Terra server.  
        /// <para>
        /// Traversal functions travel the graph assembling information based on 
        /// server-defined functions.  They are highly efficient and fast, much
        /// faster than trying to crawl the graph remotely, via the API.  If there
        /// is a case where you need information quickly, consult Brilliant Arc
        /// about creating a traversal function on the server.
        /// </para>
        /// </summary>
        /// <typeparam name="T">The results expected</typeparam>
        /// <param name="name">The name of the traversal function, typically something like edsa::taxonomy</param>
        /// <param name="start">The starting meme (category, heading, option, etc.); this may be optional, based on the traversal function</param>
        /// <returns>The results indicated by the generic</returns>
        public T Traverse<T>(String name, Meme start = null) where T : new()
        {
            var request = Request("function").AddParameter("name", name);
            if (start != null)
            {
                request.
                    AddParameter("opco", start.Opco).
                    AddParameter("definition", start.Definition).
                    AddParameter("slug", start.Slug);
            }

            return request.MakeRequest<T>();
        }

        /// <summary>
        /// Run a traversal function on the Terra server.
        /// <para>
        /// Traversal functions travel the graph assembling information based on 
        /// server-defined functions.  They are highly efficient and fast, much
        /// faster than trying to crawl the graph remotely, via the API.  If there
        /// is a case where you need information quickly, consult Brilliant Arc
        /// about creating a traversal function on the server.
        /// </para>
        /// </summary>
        /// <param name="name">The name of the traversal function, typically something like edsa::taxonomy</param>
        /// <param name="start">The starting meme (category, heading, option, etc.); this may be optional, based on the traversal function</param>
        /// <returns>The results of the function as a string; typically the results are in JSON format, but they could be XML based on the function</returns>
        public string Traverse(String name, Meme start = null)
        {
            var request = Request("function").AddParameter("name", name);
            if (start != null)
            {
                request.
                    AddParameter("opco", start.Opco).
                    AddParameter("definition", start.Definition).
                    AddParameter("slug", start.Slug);
            }

            return request.StringRequest();
        }

        /// <summary>
        /// Use the Terra server to generate an SEO-compliant slug based on the
        /// given string value.  
        /// </summary>
        /// <param name="value">The value to "slugify"</param>
        /// <returns>A slug based on the give value, with spaces and punctuation replaced with dashes</returns>
        public String Slugify(string value)
        {
            return Request("slug").AddParameter("value", value).StringRequest();
        }

        /// <summary>
        /// Generate a globally unique identifier on the server.  Sometimes this
        /// can be handy to have around.
        /// </summary>
        /// <returns>A globally unique ID, in the context of the Terra server</returns>
        public String UUID()
        {
            return Request("uuid").StringRequest();
        }

        /// <summary>
        /// Used in testing the API, when you call this you'll clear all the
        /// categories, properties, options, taxonomies, headings, superheadings,
        /// and synonyms out of the TEST portfolio.  This is not useful in a 
        /// production setting, and will return an error message if the test
        /// portfolio has not been created and initialized.
        /// </summary>
        public void ResetTestPortfolio()
        {
            Request("test/reset").StringRequest();
        }
    }
}
