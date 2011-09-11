using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terra
{
    /// <summary>
    /// A European Directories operating company, such as PKT or Fonecta.  Each
    /// operating company is identified by a three or four letter code, such as
    /// "PKT" or "FON", which is used in most requests to the server.  
    /// <para>
    /// This object is here merely as a convenience.  Typically you just pass
    /// around the operating company's slug/code.
    /// </para>
    /// </summary>
    public class OperatingCompany : Node
    {
        /// <summary>
        /// The full name of the operating company.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The three or four letter identification code for the operating
        /// company, e.g. "PKT".
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// The two character default ISO language for the operating company,
        /// such as "en" or "pl".
        /// </summary>
        public string Language { get; set; }

        public override int GetHashCode()
        {
            return ("operatingcompany:" + Slug).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            OperatingCompany o = obj as OperatingCompany;
            if ((object)o == null)
            {
                return false;
            }

            return (o.Slug == Slug);
        }

        public static OperatingCompany FromDictionary(IDictionary<string, Object> dictionary)
        {
            var opco = new OperatingCompany();
            opco.Name = dictionary["name"] as string;
            opco.Slug = dictionary["slug"] as string;
            opco.Language = dictionary["language"] as string;
            return opco;
        }
    }
}
