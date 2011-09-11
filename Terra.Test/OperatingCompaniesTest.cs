using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Terra.Test
{
    public class OperatingCompaniesTest
    {
        private Terra.Client client;

        public OperatingCompaniesTest()
        {
            client = new Terra.Client(Constants.TestServer).Authenticate(Constants.TestUser, Constants.TestPassword);
        }

        [Fact]
        public void GetOperatingCompanies()
        {
            Assert.NotEmpty(client.OperatingCompanies.All());

            var pkt = client.OperatingCompanies.Get("PKT");
            Assert.NotNull(pkt);
            Assert.Contains<Terra.OperatingCompany>(pkt, client.OperatingCompanies.All());
        }
    }
}
