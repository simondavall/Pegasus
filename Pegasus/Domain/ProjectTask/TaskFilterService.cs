using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Pegasus.Domain.Cache;
using Pegasus.Entities.Enumerations;
using Pegasus.Extensions;

namespace Pegasus.Domain.ProjectTask
{
    public interface ITaskFilterService
    {
        IEnumerable<TaskFilter> GetTaskFilters();
    }

    public class TaskFilterService : ITaskFilterService
    {
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;

        public TaskFilterService(IMemoryCache cache, IConfiguration configuration)
        {
            _cache = cache;
            _configuration = configuration;
        }

        public IEnumerable<TaskFilter> GetTaskFilters()
        {
            if (FiltersExistInCache(out var filters))
                return filters;

            filters = CreateListOfTaskFilters();
            SaveToCache(filters);

            return filters;
        }

        private bool FiltersExistInCache(out IList<TaskFilter> filters)
        {
            return _cache.TryGetValue(CacheKeys.TaskFilters, out filters);
        }

        private IList<TaskFilter> CreateListOfTaskFilters()
        {
            const TaskFilters taskValue = TaskFilters.All;
            return EnumHelper<TaskFilters>.GetValues(taskValue)
                .Select(task => new TaskFilter() { Name = EnumHelper<TaskFilters>.GetDisplayValue(task), Value = (int)task })
                .ToList();
        }

        private void SaveToCache(IList<TaskFilter> filters)
        {
            const int fallbackExpiryDays = 1;
            var expiryDays = _configuration.FromConfig("Cache:TaskFilters:ExpiryDays", fallbackExpiryDays);

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromDays(expiryDays));

            _cache.Set(CacheKeys.TaskFilters, filters, cacheEntryOptions);
        }
    }
}
