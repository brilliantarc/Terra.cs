using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Terra.Test
{
    public class OptionsTest
    {
        private Terra.Client client;

        public OptionsTest()
        {
            client = new Terra.Client(Constants.TestServer).Authenticate(Constants.TestUser, Constants.TestPassword);
        }

        // TODO:  CRUD a option
        [Fact]
        public void CrudOption()
        {
            client.ResetTestPortfolio();

            // Create a option
            Assert.DoesNotThrow(
                delegate
                {
                    client.Options.Create(Constants.TestPortfolio, "The OptionsTest Option", "test-option");
                });

            // Get the option
            var option = client.Options.Get(Constants.TestPortfolio, "test-option");
            Assert.NotNull(option);

            // Update the option
            option.Name = "The Adjusted Option Name";
            option.External = "an-external-id";
            option.Language = "nl";
            var updated = client.Options.Update(option);
            Assert.Equal(option.Slug, updated.Slug);
            Assert.Equal(option.Opco, updated.Opco);
            Assert.Equal("The Adjusted Option Name", option.Name);
            Assert.Equal("nl", option.Language);

            // Delete the option
            client.Options.Delete(updated);
            Assert.Throws<Terra.ServerException>(
                delegate
                {
                    client.Options.Get(Constants.TestPortfolio, "test-option");
                });
        }

        [Fact]
        public void allOptions()
        {
            client.ResetTestPortfolio();

            var optionA = client.Options.Create(Constants.TestPortfolio, "Option A");
            var optionB = client.Options.Create(Constants.TestPortfolio, "Option B");

            List<Option> options = client.Options.All(Constants.TestPortfolio);
            Assert.Contains<Option>(optionA, options);
            Assert.Contains<Option>(optionB, options);
        }

        // TODO:  Add an option to a category
        // TODO:  Remove an option from a category
        // TODO:  Add a synonym to a category
        // TODO:  Remove a synonym from a category
        // TODO:  Add a suboption to a category
        // TODO:  Remove a suboption from a category
    }
}
