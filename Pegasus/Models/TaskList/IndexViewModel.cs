using System;
using System.Collections.Generic;
using System.Linq;
using Pegasus.Domain.ProjectTask;
using Pegasus.Entities.Sorters.ProjectTask;
using Pegasus.Extensions;
using Pegasus.Library.Models;

namespace Pegasus.Models.TaskList
{
    public class IndexViewModel : BaseViewModel
    {
        private readonly IList<TaskModel> _filteredProjectTasks;
        private const int DefaultPageSize = 10;

        public IndexViewModel(IEnumerable<TaskModel> projectTasks, int taskFilterId)
        {
            _filteredProjectTasks = projectTasks.Filtered(taskFilterId).ToList();
            TaskFilterId = taskFilterId;
        }

        public IEnumerable<ProjectModel> Projects { get; set; }
        public ProjectModel Project { get; set; }
        public ISorter Sorter { get; set; } = new ModifiedDescSorter();
        public int TaskFilterId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = DefaultPageSize;

        public IEnumerable<TaskFilter> TaskFilters { get; set; }

        public IEnumerable<TaskModel> ProjectTasks => _filteredProjectTasks
            .Sorted(Sorter)
            .Paginated(Page, PageSize);

        public PaginationViewModel PaginationViewModel 
        {
            get {
                var paginationViewModel = new PaginationViewModel()
                {
                    Action = "Index",
                    CurrentPage = Page,
                    TotalPages = PageSize < 1 ? 0 : (int)Math.Ceiling((double)_filteredProjectTasks.Count / PageSize)
                };
                return paginationViewModel;
            }
        }
    }
}
