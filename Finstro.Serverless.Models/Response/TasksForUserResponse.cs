using System;
using System.Collections.Generic;
using System.Text;

namespace Finstro.Serverless.Models.Response
{

    public class TasksForUserResponse
    {
        public DateTime CreatedDate { get; set; }
        public string Type { get; set; }
        public BasicClient Client { get; set; }
        public string ClientStatus { get; set; }
        public decimal Amount { get; set; }
        public Representative TaskOwner { get; set; }
        public string TaskStatus { get; set; }
    }

    public class BasicClient
    {
        public int ClientID { get; set; }
        public string ClientName { get; set; }
        public string Status { get; set; }
        public decimal Limit { get; set; }
        public decimal Balance { get; set; }
        public Representative Director { get; set; }
    }

    public class Representative
    {
        public int MemberID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
    }


}
