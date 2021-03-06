﻿using System;
using System.IO.Abstractions;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Mocks.MockHttpService.Validators;
using PactNet.Models;
using PactNet.Reporters;

namespace PactNet
{
    public class PactVerifier : IPactVerifier, IProviderStates
    {
        private readonly IFileSystem _fileSystem;
        private readonly Func<HttpClient, IProviderServiceValidator> _providerServiceValidatorFactory;

        public string ConsumerName { get; private set; }
        public string ProviderName { get; private set; }
        public ProviderStates ProviderStates { get; private set; }
        public HttpClient HttpClient { get; private set; }
        public string PactFileUri { get; private set; }

        [Obsolete("For PactProvider testing only.")]
        public PactVerifier(IFileSystem fileSystem, 
            Func<HttpClient, IProviderServiceValidator> providerServiceValidatorFactory)
        {
            _fileSystem = fileSystem;
            _providerServiceValidatorFactory = providerServiceValidatorFactory;
        }

        public PactVerifier() : this(
            new FileSystem(),
            httpClient => new ProviderServiceValidator(httpClient, new Reporter()))
        {
        }

        public IProviderStates ProviderStatesFor(string consumerName, Action setUp = null, Action tearDown = null)
        {
            if (String.IsNullOrEmpty(consumerName))
            {
                throw new ArgumentException("Please supply a non null or empty consumerName");
            }

            if (!String.IsNullOrEmpty(ConsumerName) && !ConsumerName.Equals(consumerName))
            {
                throw new ArgumentException("Please supply the same consumerName that was defined when calling the HonoursPactWith method");
            }

            ConsumerName = consumerName;
            ProviderStates = new ProviderStates(setUp, tearDown);

            return this;
        }

        public IProviderStates ProviderState(string providerState, Action setUp = null, Action tearDown = null)
        {
            if (ProviderStates == null)
            {
                throw new InvalidOperationException("Please intitialise the provider states by first calling the ProviderStatesFor method");
            }

            if (String.IsNullOrEmpty(providerState))
            {
                throw new ArgumentException("Please supply a non null or empty providerState");
            }

            var providerStateItem = new ProviderState(providerState, setUp, tearDown);
            ProviderStates.Add(providerStateItem);

            return this;
        }

        public IPactVerifier ServiceProvider(string providerName, HttpClient httpClient)
        {
            if (String.IsNullOrEmpty(providerName))
            {
                throw new ArgumentException("Please supply a non null or empty providerName");
            }

            if (httpClient == null)
            {
                throw new ArgumentException("Please supply a non null httpClient");
            }

            ProviderName = providerName;
            HttpClient = httpClient;

            return this;
        }

        public IPactVerifier HonoursPactWith(string consumerName)
        {
            if (String.IsNullOrEmpty(consumerName))
            {
                throw new ArgumentException("Please supply a non null or empty consumerName");
            }

            if (!String.IsNullOrEmpty(ConsumerName) && !ConsumerName.Equals(consumerName))
            {
                throw new ArgumentException("Please supply the same consumerName that was defined when calling the ProviderStatesFor method");
            }

            ConsumerName = consumerName;

            return this;
        }

        public IPactVerifier PactUri(string uri)
        {
            if (String.IsNullOrEmpty(uri))
            {
                throw new ArgumentException("Please supply a non null or empty consumerName");
            }

            PactFileUri = uri;

            return this;
        }

        public void Verify(string providerDescription = null, string providerState = null)
        {
            if (HttpClient == null)
            {
                throw new InvalidOperationException("HttpClient has not been set, please supply a HttpClient using the ServiceProvider method.");
            }

            if (String.IsNullOrEmpty(PactFileUri))
            {
                throw new InvalidOperationException("PactFileUri has not been set, please supply a uri using the PactUri method.");
            }

            ProviderServicePactFile pactFile;
            try
            {
                var pactFileJson = _fileSystem.File.ReadAllText(PactFileUri);
                pactFile = JsonConvert.DeserializeObject<ProviderServicePactFile>(pactFileJson);
            }
            catch (System.IO.IOException)
            {
                throw new CompareFailedException(String.Format("Json Pact file could not be retrieved using uri \'{0}\'.", PactFileUri));
            }

            //Filter interactions
            if (providerDescription != null)
            {
                pactFile.Interactions = pactFile.Interactions.Where(x => x.Description.Equals(providerDescription));
            }

            if (providerState != null)
            {
                pactFile.Interactions = pactFile.Interactions.Where(x => x.ProviderState.Equals(providerState));
            }

            _providerServiceValidatorFactory(HttpClient).Validate(pactFile, ProviderStates);
        }
    }
}
