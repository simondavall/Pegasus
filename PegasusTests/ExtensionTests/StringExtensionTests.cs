using NUnit.Framework;
using Pegasus.Extensions;

namespace PegasusTests.ExtensionTests
{
    class StringExtensionTests
    {
        [Test]
        public void Linkify_NoUrlMatch_ReturnsUnchangedText()
        {
            var originalText = "Some text without a web url";
            var sut = originalText.Linkify();

            Assert.AreEqual(originalText, sut);
        }

        [Test]
        public void Linkify_OneUrlMatch_ReturnsTextWithHyperlink()
        {
            var originalText = "Lorem ipsum dolor sit amet, consectetur https://example.com adipiscing elit. Nunc consectetur elementum interdum.";
            var sut = originalText.Linkify();

            Assert.AreEqual("Lorem ipsum dolor sit amet, consectetur <a href=\"https://example.com\">example.com</a> adipiscing elit. Nunc consectetur elementum interdum.", sut);
        }

        [Test]
        public void Linkify_TwoUrlMatches_ReturnsTextWithTwoHyperlinks()
        {
            var originalText = "Lorem ipsum dolor sit amet, consectetur https://www.example.com adipiscing elit. Nunc consectetur https://example.com elementum interdum.";
            var sut = originalText.Linkify();

            Assert.AreEqual("Lorem ipsum dolor sit amet, consectetur <a href=\"https://www.example.com\">www.example.com</a> adipiscing elit. Nunc consectetur <a href=\"https://example.com\">example.com</a> elementum interdum.", sut);
        }

        [Test]
        public void Linkify_UrlWithQueryString_ReturnsTextWithHyperlinkWithQueryString()
        {
            var originalText = "Lorem ipsum dolor sit amet, consectetur https://www.example.com/area/region?id=13241234 adipiscing elit. Nunc consectetur elementum interdum.";
            var sut = originalText.Linkify();

            Assert.AreEqual("Lorem ipsum dolor sit amet, consectetur <a href=\"https://www.example.com/area/region?id=13241234\">www.example.com/area/region?id=13241234</a> adipiscing elit. Nunc consectetur elementum interdum.", sut);
        }

        [Test]
        public void Linkify_UrlWithLinkifiedLink_ReturnsUnchangedString()
        {
            var originalText = "Lorem ipsum dolor sit amet, consectetur <a href=\"https://example.com\">example.com</a> adipiscing elit. Nunc consectetur elementum interdum.";
            var sut = originalText.Linkify();

            Assert.AreEqual("Lorem ipsum dolor sit amet, consectetur <a href=\"https://example.com\">example.com</a> adipiscing elit. Nunc consectetur elementum interdum.", sut);
        }

        // https://localhost:5002/TaskList/Edit/14027

        [Test]
        public void Linkify_UrlWithPort_ReturnsCorrectHyperlink()
        {
            var originalText = "Lorem ipsum dolor sit amet, consectetur https://localhost:5002/TaskList/Edit/14027 adipiscing elit. Nunc consectetur elementum interdum.";
            var sut = originalText.Linkify();

            Assert.AreEqual("Lorem ipsum dolor sit amet, consectetur <a href=\"https://localhost:5002/TaskList/Edit/14027\">localhost:5002/TaskList/Edit/14027</a> adipiscing elit. Nunc consectetur elementum interdum.", sut);
        }


    }
}
