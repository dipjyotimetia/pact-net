﻿using System;
using System.Text;
using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService.Models
{
    public class HttpBodyContentTests
    {
        [Fact]
        public void Ctor1_WithNullBody_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new HttpBodyContent(body: null, contentType: null, encoding: null));
        }

        [Fact]
        public void Ctor2_WithNullContent_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new HttpBodyContent(content: null, contentType: null, encoding: null));
        }

        [Fact]
        public void Ctor1_WithContentType_SetsContentType()
        {
            var contentTypeString = "text/html";
            var httpBodyContent = new HttpBodyContent(body: new {}, contentType: contentTypeString, encoding: null);

            Assert.Equal(contentTypeString, httpBodyContent.ContentType);
        }

        [Fact]
        public void Ctor2_WithContentType_SetsContentType()
        {
            var contentTypeString = "text/html";
            var httpBodyContent = new HttpBodyContent(content: String.Empty, contentType: contentTypeString, encoding: null);

            Assert.Equal(contentTypeString, httpBodyContent.ContentType);
        }

        [Fact]
        public void Ctor1_WithEncoding_SetsEncoding()
        {
            var encoding = Encoding.Unicode;
            var httpBodyContent = new HttpBodyContent(body: new { }, contentType: null, encoding: encoding);

            Assert.Equal(encoding, httpBodyContent.Encoding);
        }

        [Fact]
        public void Ctor2_WithEncoding_SetsEncoding()
        {
            var encoding = Encoding.Unicode;
            var httpBodyContent = new HttpBodyContent(body: String.Empty, contentType: null, encoding: encoding);

            Assert.Equal(encoding, httpBodyContent.Encoding);
        }

        [Fact]
        public void Ctor1_WithJsonBody_SetsBodyAndContent()
        {
            var body = new
            {
                Test = "tester",
                tesTer = 1
            };
            const string content = "{\"Test\":\"tester\",\"tesTer\":1}";
            var httpBodyContent = new HttpBodyContent(body: body, contentType: "application/json", encoding: null);

            Assert.Equal(content, httpBodyContent.Content);
            Assert.Equal(body, httpBodyContent.Body);
        }

        [Fact]
        public void Ctor2_WithJsonContent_SetsBodyAndContent()
        {
            var body = new
            {
                Test = "tester",
                tesTer = 1
            };
            const string content = "{\"Test\":\"tester\",\"tesTer\":1}";
            var httpBodyContent = new HttpBodyContent(content: content, contentType: "application/json", encoding: null);

            Assert.Equal(content, httpBodyContent.Content);
            Assert.Equal(body.Test, (string)httpBodyContent.Body.Test);
            Assert.Equal(body.tesTer, (int)httpBodyContent.Body.tesTer);
        }

        [Fact]
        public void Ctor1_WithPlainTextBody_SetsBodyAndContent()
        {
            const string body = "Some plain text";
            var httpBodyContent = new HttpBodyContent(body: body, contentType: "application/plain", encoding: null);

            Assert.Equal(body, httpBodyContent.Content);
            Assert.Equal(body, httpBodyContent.Body);
        }

        [Fact]
        public void Ctor2_WithPlainTextContent_SetsBodyAndContent()
        {
            const string content = "Some plain text";
            var httpBodyContent = new HttpBodyContent(content: content, contentType: "application/plain", encoding: null);

            Assert.Equal(content, httpBodyContent.Content);
            Assert.Equal(content, httpBodyContent.Body);
        }

        [Fact]
        public void ContentBytes_WithEmptyContent_ReturnsEmptyUtf8ByteArray()
        {
            var httpBodyContent = new HttpBodyContent(String.Empty, null, null);

            Assert.Empty(httpBodyContent.ContentBytes);
        }

        [Fact]
        public void ContentType_WithNullContentTypeSet_ReturnsPlainContentType()
        {
            var httpBodyContent = new HttpBodyContent(new { }, null, null);

            Assert.Equal("text/plain", httpBodyContent.ContentType);
        }

        [Fact]
        public void ContentType_WithEmptyContentTypeSet_ReturnsPlainContentType()
        {
            var httpBodyContent = new HttpBodyContent(new { }, String.Empty, null);

            Assert.Equal("text/plain", httpBodyContent.ContentType);
        }

        [Fact]
        public void Encoding_WithNullEncodingSet_ReturnsUtf8Encoding()
        {
            var httpBodyContent = new HttpBodyContent(new {}, null, null);

            Assert.Equal(Encoding.UTF8, httpBodyContent.Encoding);
        }
    }
}
