using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terra
{
    /// <summary>
    /// The Terra 2.0 Client API allows developers to build software applications
    /// locally that can interact with the Brilliant Arc Terra Taxonomy 
    /// Management Tool on an active and ongoing basis, with the same level of
    /// functionality as the Terra Client UI.  This enables local back-office
    /// and consumer-driven applications to utilitze the knowledge contained 
    /// within Terra as they would any other database, but with the added power
    /// of the Terra relations and inheritance technologies.
    /// <para>
    /// The API is a very lightweight, thin wrapper around the RESTful JSON-based
    /// server API.  Most of the business logic, authentication, authorization
    /// and other functionality resides on the server.  The client API does 
    /// contain a few bits of helper code, but it is not a full-blown object
    /// model system.  It is simple, safe, and fast.
    /// </para>
    /// <para>
    /// The Terra 2.0 Client API has been redesigned to conform with C# coding
    /// standards.  It is also somewhat functional in nature, due to its 
    /// interactions with the server:  no objects are ever directly modified
    /// by the API.  For example, updating a category will return a new
    /// instance of a Category object; it will not change the category object
    /// submitted. 
    /// </para>
    /// <para>
    /// Functions of the API are all handled on the Client services, rather 
    /// than the objects themselves.  This keeps the library both simple and
    /// thread-safe.  For example, when mapping a heading to a category, you
    /// call a function on Terra.Client.Headings, not on the Terra.Heading
    /// object.  This reinforces the functional nature of the API.
    /// </para>
    /// <para>
    /// To make a request of the server, first authenticate a user against an
    /// instance of the Terra.Client, then submit requests across that client's
    /// services.  
    /// </para>
    /// <code>
    ///     var client = new Terra.Client("http://tax.eurodir.eu/api");
    ///     client.authenticate("someuser", "somepassword");
    ///     List&lt;Taxonomy&gt; taxonomies = client.Taxonomies.All("PKT");
    /// </code>
    /// <para>
    /// This API represents the current and complete functionality available to
    /// the Terra Client Application (UI).  As new features are added to that
    /// application, they will become available in this API as well.  In fact,
    /// the Terra Client UI itself uses the same calls as the API--we are
    /// "eating our own dog food", as it were.
    /// </para>
    /// </summary>

    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc
    {
    }
}
