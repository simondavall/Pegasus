using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pegasus.Entities.Sorters.ProjectTask;
using Pegasus.Extensions;
using Pegasus.Library.Models;
using Pegasus.Services.Models;

namespace Pegasus.Models.TaskList
{
    public class IndexViewModel : BaseViewModel
    {
        private readonly SettingsModel _settingsModel;
        private readonly IList<TaskModel> _filteredProjectTasks;
        private const int DefaultPageSize = 10;

        public IndexViewModel(IEnumerable<TaskModel> projectTasks, int taskFilterId, SettingsModel settingsModel)
        {
            _settingsModel = settingsModel;
            _filteredProjectTasks = projectTasks.Filtered(taskFilterId).ToList();
            TaskFilterId = taskFilterId;
            PaginationEnabled = settingsModel.PaginationEnabled;
        }

        public IEnumerable<ProjectModel> Projects { get; set; }
        public ProjectModel Project { get; set; }
        public ISorter Sorter { get; set; } = new ModifiedDescSorter();
        public int TaskFilterId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = DefaultPageSize;
        public bool PaginationEnabled { get; set; }

        public IEnumerable<SelectListItem> ProjectListItems { get; set; }
        public IEnumerable<SelectListItem> TaskFilterListItems { get; set; }

        public ProjectSummaryViewModel ProjectSummaryViewModel { get; set; }

        public IEnumerable<TaskFilterModel> TaskFilters { get; set; }

        public IEnumerable<TaskModel> ProjectTasks => _filteredProjectTasks
            .Sorted(Sorter)
            .Paginated(Page, PageSize, PaginationEnabled);

        public PaginationViewModel PaginationViewModel 
        {
            get {
                var paginationViewModel = new PaginationViewModel(_settingsModel)
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
