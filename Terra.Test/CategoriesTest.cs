using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Terra.Test
{
    public class CategoriesTest
    {
        private Terra.Client client;

        public CategoriesTest()
        {
            client = new Terra.Client(Constants.TestServer).Authenticate(Constants.TestUser, Constants.TestPassword);
        }

        [Fact]
        public void CrudCategories()
        {
            client.ResetTestPortfolio();

            // Create a category
            Assert.DoesNotThrow(
                delegate
                {
                    client.Categories.Create(Constants.TestPortfolio, "The CategoriesTest Category", "test-category");
                });

            // Get the category
            var category = client.Categories.Get(Constants.TestPortfolio, "test-category");
            Assert.NotNull(category);

            // Update the category
            category.Name = "The Adjusted Category Name";
            category.External = "an-external-id";
            category.Language = "nl";
            var updated = client.Categories.Update(category);
            Assert.Equal(category.Slug, updated.Slug);
            Assert.Equal(category.Opco, updated.Opco);
            Assert.Equal("The Adjusted Category Name", category.Name);
            Assert.Equal("nl", category.Language);

            // Delete the category
            client.Categories.Delete(updated);
            Assert.Throws<Terra.ServerException>(
                delegate
                {
                    client.Categories.Get(Constants.TestPortfolio, "test-category");
                });
        }

        [Fact]
        public void CategorySynonyms()
        {
            client.ResetTestPortfolio();

            var category = client.Categories.Create(Constants.TestPortfolio, "A Sample Category", "test-category");

            // Try creating a synonym
            var synonym = client.Categories.CreateSynonym(category, "A Sample Synonym");

            // See if it attached properly
            List<Synonym> synonyms = client.Categories.Synonyms(category);
            Assert.Contains<Terra.Synonym>(synonym, client.Categories.Synonyms(category));

            // Remove the synonym
            client.Categories.RemoveSynonym(category, synonym);
            Assert.DoesNotContain<Terra.Synonym>(synonym, client.Categories.Synonyms(category));
        }

        [Fact]
        public void RelateHeadings()
        {
            client.ResetTestPortfolio();

            // Create a category
            var category = client.Categories.Create(Constants.TestPortfolio, "A Sample Category", "test-category");

            // Create a heading
            var heading = client.Headings.Create(Constants.TestPortfolio, "A Sample Heading", "test-heading");

            // Relate the heading to the category
            client.Headings.MapHeading(heading, category);

            // See if it's related
            Assert.Contains<Terra.Category>(category, client.Headings.MappedTo(heading));
            Assert.Contains<Terra.Heading>(heading, client.Categories.MappedHeadings(category));
        }

        [Fact]
        public void RelateCategories()
        {
            client.ResetTestPortfolio();

            // Create two categories
            var categoryA = client.Categories.Create(Constants.TestPortfolio, "Category A");
            var categoryB = client.Categories.Create(Constants.TestPortfolio, "Category B");

            // Relate one to the other
            client.Categories.MapCategory(categoryA, categoryB);

            // Get the mapped categories and see if it's there
            Assert.Contains<Terra.Category>(categoryB, client.Categories.MappedTo(categoryA));
            Assert.Contains<Terra.Category>(categoryA, client.Categories.MappedFrom(categoryB));
        }

        [Fact]
        public void Inheritance()
        {
            client.ResetTestPortfolio();

            // Create a parent category
            var parent = client.Categories.Create(Constants.TestPortfolio, "Parent Category");

            // Create a child category
            var child = client.Categories.Create(Constants.TestPortfolio, "Child Category");
            client.Categories.AddChild(parent, child);

            // Attach a property and option to the parent
            var cuisine = client.Properties.Create(Constants.TestPortfolio, "Cuisine");
            var seafood = client.Options.Create(Constants.TestPortfolio, "Seafood");
            client.Categories.AddOption(parent, cuisine, seafood);

            // Attach a property and option to the child
            var hamburger = client.Options.Create(Constants.TestPortfolio, "Hamburger");
            client.Categories.AddOption(child, cuisine, hamburger);

            // Get the child category's properties and options
            List<Property> properties = client.Categories.Options(child);
            Assert.Equal(1, properties.Count);
            Assert.NotNull(properties[0]);
            Assert.NotNull(properties[0].Options);

            //// Should contain just the directly attached property and option
            Assert.Contains<Terra.Option>(hamburger, properties[0].Options);
            Assert.DoesNotContain<Terra.Option>(seafood, properties[0].Options);

            //// Get the child category's inheritance
            List<Property> inherited = client.Categories.Inheritance(child);
            Assert.Equal(1, inherited.Count);
            Assert.NotNull(inherited[0].Options);

            //// Should contain both parent and child category
            Assert.Contains<Terra.Option>(hamburger, inherited[0].Options);
            Assert.Contains<Terra.Option>(seafood, inherited[0].Options);
        }
    }
}
