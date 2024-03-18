using System.ComponentModel;
//using Finstro.Serverless.Common;

namespace Finstro.Serverless.Models.Request
{
    /// <summary>Search Request Model</summary>
	public class SearchRequestModel
	{
        /// <summary>Text to search</summary>
		public string Search { get; set; }

        /// <summary>
        /// Accepted values are:
        ///     Abn, CompanyName, CompanyLegalName, FirstName, LastName, Email, Mobile
        ///
        /// </summary>
		public string[] Fields { get; set; }
	}

}
