using System;
using System.Collections.Generic;

namespace Pegasus.Models.TaskList
{
    public class ProjectSummaryViewModel
    {
        public string ProjectName { get; set; }
        public DateTime ProjectCreated { get; set; }
        public DateTime ProjectLastAction { get; set; }
        public int TotalIssues { get; set; }
        public int OpenIssues { get; set; }
        public int ClosedIssues { get; set; }
        public int BacklogIssues { get; set; }

        public string CurrentVersion { get; set; }
        public IEnumerable<ProjectContributor> ProjectContributors { get; set; }
    }

    public class ProjectContributor
    {
        public string Name { get; set; }
        public int NumberOfContributions { get; set; }
        public DateTime LastContribution { get; set; }
    }
}
