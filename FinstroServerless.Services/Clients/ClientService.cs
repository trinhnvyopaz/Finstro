using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Finstro.Serverless.Dapper.Repository;
using Finstro.Serverless.Models.Request;
using Finstro.Serverless.Models.Response;
using Finstro.Serverless.Helper;
using Finstro.Serverless.Models;

namespace FinstroServerless.Services.Clients
{
    public class ClientService
    {
        private ClientRepository _clientRepository;

        public ClientService()
        {
            _clientRepository = new ClientRepository();
        }

        public List<ClientListResponse> GetClientList(SearchRequestModel search)
        {
            try
            {
                var enumFields = Enum.GetValues(typeof(SearchFields)).Cast<SearchFields>().ToArray();
                List<string> fields = new List<string>();

                try
                {
                    var items = search.Fields.Select(a => (SearchFields)Enum.Parse(typeof(SearchFields), a)).ToList();


                    foreach (var item in items)
                    {
                        fields.Add(item.GetAttributeOfType<DescriptionAttribute>().Description);
                    }
                }
                catch
                {
                    throw new NotFoundCustomException("Invalid Search Field", $"Possible Values: {string.Join(", ", enumFields)}");
                }

                if (string.IsNullOrEmpty(search.Search))
                    search.Search = string.Empty;

                List<ClientListResponse> list = _clientRepository.GetClientList(search.Search, fields.ToArray()).ToList();


                return list;            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }
    }

}
