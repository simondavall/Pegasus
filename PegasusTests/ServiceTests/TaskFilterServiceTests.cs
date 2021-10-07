using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Pegasus.Domain.Cache;
using Pegasus.Entities.Enumerations;
using Pegasus.Services;
using Pegasus.Services.Models;

namespace PegasusTests
{
    class TaskFilterServiceTests
    {
        private IConfiguration _configuration;
        private IMemoryCache _cache;

        [SetUp]
        public void Setup()
        {
            var myJsonConfig = "{   \"Cache\": { \"TaskFilters\": { \"ExpiryDays\": \"3\" } }}";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(myJsonConfig));

            _configuration = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();


            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        [Test]
        public void GetAllFilters_ReturnsAllTaskFilters()
        {
            var sut = new TaskFilterService(_cache,_configuration);
            var result = sut.GetTaskFilters();

            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Count());
        }

        [Test]
        public void GetAllFilters_SavesFiltersToCache()
        {
            var sut = new TaskFilterService(_cache, _configuration);
            var result = sut.GetTaskFilters();

            var cachedEntry = _cache.Get<IEnumerable<TaskFilterModel>>(CacheKeys.TaskFilters);

            Assert.IsNotEmpty(cachedEntry);
            Assert.AreEqual(result, cachedEntry);
        }

        [Test]
        public void GetAllFilters_CacheExpiryComesFromSettings()
        {
            var sut = new TaskFilterService(_cache, _configuration);
            sut.GetTaskFilters();

            var cachedExpiryDays = _cache.Get<int>(CacheKeys.TaskFiltersExpiryDays);
            Assert.AreEqual(3, cachedExpiryDays);
        }

        [Test]
        public void GetAllFilters_CacheExpiryUsesFallbackWhenExpirySettingNotFound()
        {
            var myJsonConfig = "{   \"Cache\": { \"TaskFilters\": { \"INCORRECT\": \"3\" } }}";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(myJsonConfig));

            var configuration = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();

            var sut = new TaskFilterService(_cache, configuration);
            sut.GetTaskFilters();

            var cachedExpiryDays = _cache.Get<int>(CacheKeys.TaskFiltersExpiryDays);
            Assert.AreEqual(1, cachedExpiryDays);
        }

        [Test]
        public void GetTaskFilters_ExistInCache_ReturnsCachedVersion()
        {
            var taskFilters = Enum.GetValues(typeof(TaskFilters));
            var count = taskFilters.Length;

            var sut = new TaskFilterService(_cache,_configuration);
            var result = sut.GetTaskFilters();

            Assert.AreEqual(count, result.Count());

            var list = result.ToList();
            list.RemoveAt(0);
            // remove an item and save back to cache
            _cache.Set(CacheKeys.TaskFilters, list);
            // call the 'get' again to see if we get 4 or 5 items
            var cachedResult = sut.GetTaskFilters();

            Assert.AreEqual(count - 1, cachedResult.Count());
        }
    }
}
