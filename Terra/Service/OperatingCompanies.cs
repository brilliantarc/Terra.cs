using System;
using System.Collections.Generic;
using RestSharp;

namespace Terra.Service
{
    /// <summary>
    /// API calls relating to operating companies.
    /// <para>
    /// Do not create an instance of this directly.  Instead, call in through 
    /// the Terra.Client.OperatingCompanies.
    /// </para>
    /// </summary>
    /// <seealso cref="Terra.Client.OperatingCompanies"/>
    public class OperatingCompanies
    {
        private Client _client;

        public OperatingCompanies(Client client)
        {
            _client = client;
        }

        /// <summary>
        /// Retrieve the available operating companies from Terra.
        /// </summary>
        /// <returns>A list of operating companies</returns>
        public List<OperatingCompany> All()
        {
            return _client.Request("opcos").MakeRequest<List<OperatingCompany>>();
        }

        /// <summary>
        /// Retrieve the details about an operating company, such as its name 
        /// and default language.
        /// </summary>
        /// <param name="opco">The three or four letter identifier for the operating company</param>
        /// <returns>The operating company</returns>
        /// <exception cref="Terra.ServerException">
        /// An operating company does not exist for the given slug
        /// </exception>
        public OperatingCompany Get(string opco)
        {
            return _client.Request("opco").AddParameter("opco", opco).MakeRequest<OperatingCompany>();
        }

        /// <summary>
        /// Convenience method to return all the taxonomies for an operating
        /// company.
        /// </summary>
        /// <seealso cref="Terra.Service.Taxonomies.All"/>
        /// <param name="opco">The three or four letter operating company code</param>
        /// <returns>A list of taxonomies</returns>
        /// <exception cref="Terra.ServerException">The operating company does not exist</exception>
        public List<Taxonomy> Taxonomies(string opco)
        {
            return _client.Taxonomies.All(opco);
        }

        /// <summary>
        /// Convenience method to return all the properties for an operating
        /// company.
        /// </summary>
        /// <seealso cref="Terra.Service.Properties.All"/>
        /// <param name="opco">The three or four letter operating company code</param>
        /// <returns>A list of properties</returns>
        /// <exception cref="Terra.ServerException">The operating company does not exist</exception>
        public List<Property> Properties(string opco)
        {
            return _client.Properties.All(opco);
        }

        /// <summary>
        /// Convenience method to return all the superheadings for an operating
        /// company.
        /// </summary>
        /// <seealso cref="Terra.Service.Superheadings.All"/>
        /// <param name="opco">The three or four letter operating company code</param>
        /// <returns>A list of superheadings</returns>
        /// <exception cref="Terra.ServerException">The operating company does not exist</exception>
        public List<Superheading> Superheadings(string opco)
        {
            return _client.Superheadings.All(opco);
        }

        /// <summary>
        /// Convenience method to return all the headings for an operating
        /// company.
        /// </summary>
        /// <seealso cref="Terra.Service.Headings.All"/>
        /// <param name="opco">The three or four letter operating company code</param>
        /// <returns>A list of headings</returns>
        /// <exception cref="Terra.ServerException">The operating company does not exist</exception>
        public List<Heading> Headings(string opco)
        {
            return _client.Headings.All(opco);
        }

        /// <summary>
        /// Add a user to the operating company.  This will grant the user 
        /// write-access to information associated with the opco.
        /// </summary>
        /// <param name="opco">The three or four letter operating company code</param>
        /// <param name="user">The user account to associate with the opco</param>
        public void AddUser(string opco, User user)
        {
            _client.Request("opco/user", Method.PUT).
                AddParameter("opco", opco).
                AddParameter("login", user.Login).
                MakeRequest();
        }

        /// <summary>
        /// Remove a user from the operating company.  This will deny the user
        /// write-access to the content in that opco's portfolio.
        /// </summary>
        /// <param name="opco">The three or four letter operating company code</param>
        /// <param name="user">The user account to disassociate with the opco</param>
        public void RemoveUser(string opco, User user)
        {
            _client.Request("opco/user", Method.DELETE).
                AddParameter("opco", opco).
                AddParameter("login", user.Login).
                MakeRequest();
        }

        /// <summary>
        /// Get the history of changes made across the entire operating company,
        /// to every category, property, option, taxonomy, heading and even
        /// superheading.
        /// </summary>
        /// <param name="opco">The three or four letter code for the operating company</param>
        /// <param name="from">The starting result to return; defaults to the beginning (0)</param>
        /// <param name="max">The maximum number of results to return; defaults to 100</param>
        /// <returns>The list of changes made to each piece of information in Terra, for this opco</returns>
        public List<String> History(string opco, int from = 0, int max = 100)
        {
            return _client.Request("opco/history").
                AddParameter("opco", opco).
                AddParameter("from", from).
                AddParameter("max", max).
                MakeRequest<List<String>>();
        }
    }
}
