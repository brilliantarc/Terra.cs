using System;
using System.Collections.Generic;
using RestSharp;
using Newtonsoft.Json;

namespace Terra
{
    /// <summary>
    /// Wraps the RestSharp.RestRequest functionality in something a little
    /// simpler and streamlined for our purposes.  For example, adds the 
    /// JSON headers, and the user credentials if present in the client. 
    /// <para>
    /// Do not instantiate an instance of this class directly; get it from
    /// the Terra.Client.
    /// </para>
    /// </summary>
    public class Request
    {
        private Client _client;
        private RestRequest _request;

        public RestRequest Base
        {
            get { return _request; }
            protected set { }
        }

        /// <summary>
        /// Used by the Terra.Client to create a new request.
        /// </summary>
        /// <param name="client">The Client instance with which this request is associated</param>
        /// <param name="resource">The path to the Terra server information, e.g. "categories"</param>
        /// <param name="method">The HTTP method type of the request; defaults to Method.GET</param>
        public Request(Client client, string resource, Method method = Method.GET)
        {
            _client = client;

            _request = new RestRequest(resource, method);
            _request.AddHeader("Accepts", "application/json");

            if (_client.User != null && _client.User.UserCredentials != null)
            {
                _request.AddParameter("user_credentials", _client.User.UserCredentials);
            }
        }

        /// <summary>
        /// Calls the RestRequest.AddParameter method if a value is submitted.  
        /// Otherwise skips it.  
        /// </summary>
        /// <param name="name">The parameter name to set</param>
        /// <param name="value">The value to set it to in the request</param>
        /// <returns>This Request object, so you can chain AddParameter calls together</returns>
        public Request AddParameter(string name, Object value)
        {
            if (value != null)
                _request.AddParameter(name, value);

            return this;
        }

        /// <summary>
        /// Wraps the call to RestClient.Execute, so that we can trap and throw exceptions properly.
        /// </summary>
        /// <typeparam name="T">The expected return type values, such as Category or List&lt;Option&gt;</typeparam>
        /// <returns>The results of the request</returns>
        public T MakeRequest<T>() where T : new()
        {
            RestResponse<T> result = _client.Base.Execute<T>(_request);
            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                if (result.Content == null)
                {
                    throw new ServerException(result.ErrorMessage, System.Net.HttpStatusCode.InternalServerError);
                }
                else
                {
                    try
                    {
                        IDictionary<string, Object> json = JsonConvert.DeserializeObject<IDictionary<string, Object>>(result.Content);
                        string message = (string)json["error"];

                        // Handle deduping...
                        if (json.ContainsKey("duplicate"))
                        {
                            object temp = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(json["duplicate"]));
                            throw new ServerException(message, result.StatusCode, (Node)temp);
                        }
                        else
                        {
                            throw new ServerException(message, result.StatusCode);
                        }
                    }
                    catch (NullReferenceException)
                    {
                        throw new ServerException(result.Content, System.Net.HttpStatusCode.InternalServerError);
                    }
                }
            }
            else
            {
                return result.Data;
            }
        }

        /// <summary>
        /// Make a Terra API request that may return a mixture of Node types,
        /// such as search results.
        /// </summary>
        /// <returns>A list of varying types of nodes</returns>
        public List<Node> GenericRequest()
        {
            List<Node> results = new List<Node>();

            RestResponse result = _client.Base.Execute(_request);
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                // Convert the JSON that comes back into the correct Node types
                IList<IDictionary<string, Object>> json = JsonConvert.DeserializeObject<IList<IDictionary<string, Object>>>(result.Content);
                foreach (var node in json)
                {
                    Console.WriteLine("Examining node " + node["name"] + ", " + node["definition"]);
                    try
                    {
                        switch (node["definition"] as String)
                        {
                            case "OperatingCompany":
                                results.Add(OperatingCompany.FromDictionary(node));
                                break;
                            case "Taxonomy":
                                results.Add(Taxonomy.FromDictionary(node));
                                break;
                            case "Category":
                                results.Add(Category.FromDictionary(node));
                                break;
                            case "Property":
                                results.Add(Property.FromDictionary(node));
                                break;
                            case "Option":
                                results.Add(Option.FromDictionary(node));
                                break;
                            case "Synonym":
                                results.Add(Synonym.FromDictionary(node));
                                break;
                            case "Heading":
                                results.Add(Heading.FromDictionary(node));
                                break;
                            case "Superheading":
                                results.Add(Superheading.FromDictionary(node));
                                break;
                        }
                    }
                    catch (KeyNotFoundException e) {
                        Console.WriteLine(e);
                    }
                }
            }
            else
            {
                if (result.Content == null)
                {
                    throw new ServerException(result.ErrorMessage, System.Net.HttpStatusCode.InternalServerError);
                }
                else
                {
                    try
                    {
                        IDictionary<string, Object> json = JsonConvert.DeserializeObject<IDictionary<string, Object>>(result.Content);
                        string message = (string)json["error"];
                        throw new ServerException(message, result.StatusCode);
                    }
                    catch (NullReferenceException)
                    {
                        throw new ServerException(result.Content, System.Net.HttpStatusCode.InternalServerError);
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Just return the string content from the HTTP request.
        /// </summary>
        /// <returns>A string value received from the server</returns>
        public String StringRequest()
        {
            RestResponse result = _client.Base.Execute(_request);
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return result.Content;
            }
            else
            {
                if (result.Content == null)
                {
                    throw new ServerException(result.ErrorMessage, System.Net.HttpStatusCode.InternalServerError);
                }
                else
                {
                    try
                    {
                        IDictionary<string, Object> json = JsonConvert.DeserializeObject<IDictionary<string, Object>>(result.Content);
                        string message = (string)json["error"];
                        throw new ServerException(message, result.StatusCode);
                    }
                    catch (NullReferenceException)
                    {
                        throw new ServerException(result.Content, System.Net.HttpStatusCode.InternalServerError);
                    }
                }
            }
        }

        /// <summary>
        /// Wraps the call to RestClient.Execute, so that we can trap and 
        /// throw exceptions properly. This type of request doesn't expect a 
        /// response, just an error if something goes wrong.
        /// </summary>
        public void MakeRequest()
        {
            RestResponse result = _client.Base.Execute(_request);
            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                if (result.Content == null)
                {
                    throw new ServerException(result.ErrorMessage, System.Net.HttpStatusCode.InternalServerError);
                }
                else
                {
                    try
                    {
                        IDictionary<string, Object> json = JsonConvert.DeserializeObject<IDictionary<string, Object>>(result.Content);
                        string message = (string)json["error"];
                        throw new ServerException(message, result.StatusCode);
                    }
                    catch (NullReferenceException)
                    {
                        throw new ServerException(result.Content, System.Net.HttpStatusCode.InternalServerError);
                    }
                }
            }
        }

    }
}
