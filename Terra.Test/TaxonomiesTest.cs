using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Terra.Test
{
    public class TaxonomiesTest
    {
        private Terra.Client client;

        public TaxonomiesTest()
        {
            client = new Terra.Client(Constants.TestServer).Authenticate(Constants.TestUser, Constants.TestPassword);
        }

        [Fact]
        public void CrudTaxonomies()
        {
            client.ResetTestPortfolio();

            // Create a taxonomy
            Assert.DoesNotThrow(
                delegate
                {
                    client.Taxonomies.Create(Constants.TestPortfolio, "The CRUD Test Taxonomy", "crud-test", "en");
                });
            Assert.NotEmpty(client.Taxonomies.All(Constants.TestPortfolio));

            // Get the taxonomy
            var taxonomy = client.Taxonomies.Get(Constants.TestPortfolio, "crud-test");
            Assert.NotNull(taxonomy);
            Assert.Contains<Terra.Taxonomy>(taxonomy, client.Taxonomies.All(Constants.TestPortfolio));

            // Update the taxonomy
            taxonomy.Name = "The Adjusted CRUD Test Taxonomy";
            taxonomy.Language = "nl";
            var updated = client.Taxonomies.Update(taxonomy);
            Assert.Equal(taxonomy.Slug, updated.Slug);
            Assert.Equal(taxonomy.Opco, updated.Opco);
            Assert.Equal("The Adjusted CRUD Test Taxonomy", taxonomy.Name);
            Assert.Equal("nl", taxonomy.Language);

            // Delete the taxonomy
            client.Taxonomies.Delete(updated);
            Assert.DoesNotContain<Terra.Taxonomy>(updated, client.Taxonomies.All(Constants.TestPortfolio));
            Assert.Throws<Terra.ServerException>(
                delegate
                {
                    client.Taxonomies.Get(Constants.TestPortfolio, "crud-test");
                });
        }
    }
}

