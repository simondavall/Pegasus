using System;
using System.Web;
using NUnit.Framework;
using Pegasus.Extensions;

namespace PegasusTests.ExtensionTests
{
    class StringExtensionTests
    {
        [Test]
        public void PreparedHtml_WithNullString_ReturnsNull()
        {
            var sut = ((string)null).PreparedHtml();

            Assert.IsNull(sut);
        }

        [Test]
        public void PreparedHtml_WithTaggedString_ReturnsEncodedString()
        {
            var unsafeText = "<script>alert('Some malevolent script')</script>";
            
            var sut = unsafeText.PreparedHtml();

            Assert.AreNotEqual(unsafeText, sut);
            Assert.AreEqual(HttpUtility.HtmlEncode(unsafeText), sut);
        }

        [Test]
        public void PreparedHtml_WithNewLineString_ReturnsBRtaggedString()
        {
            var unsafeText = "<script>alert('Some malevolent script')</script> " + Environment.NewLine + "Some more text.";
            
            var sut = unsafeText.PreparedHtml();

            Assert.AreNotEqual(unsafeText, sut);
            Assert.AreNotEqual(-1, sut.IndexOf("<br/>", StringComparison.Ordinal));
        }

        [Test]
        public void PreparedHtml_WithWebLinkString_ReturnsAnchorTaggedString()
        {
            var unsafeText = "<script>alert('Some malevolent script')</script> and a web link https://example.com" + Environment.NewLine + "Some more text.";
            
            var sut = unsafeText.PreparedHtml();

            Assert.AreNotEqual(unsafeText, sut);
            Assert.AreNotEqual(-1, sut.IndexOf("<a href=\"https://example.com\">", StringComparison.Ordinal));
        }



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

        [Test]
        public void Linkify_StringIsNull_ReturnsNull()
        {
            string originalText = null;
            // ReSharper disable once ExpressionIsAlwaysNull
            var sut = originalText.Linkify();

            Assert.IsNull(sut);
        }

    }
}
