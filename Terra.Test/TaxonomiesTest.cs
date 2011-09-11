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
            // Create a taxonomy
            Assert.DoesNotThrow(
                delegate
                {
                    client.Taxonomies.Create("PKT", "The CRUD Test Taxonomy", "crud-test", "en");
                });
            Assert.NotEmpty(client.Taxonomies.All("PKT"));

            // Get the taxonomy
            var taxonomy = client.Taxonomies.Get("PKT", "crud-test");
            Assert.NotNull(taxonomy);
            Assert.Contains<Terra.Taxonomy>(taxonomy, client.Taxonomies.All("PKT"));

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
            Assert.DoesNotContain<Terra.Taxonomy>(updated, client.Taxonomies.All("PKT"));
            Assert.Throws<Terra.ServerException>(
                delegate
                {
                    client.Taxonomies.Get("PKT", "crud-test");
                });
        }
    }
}

