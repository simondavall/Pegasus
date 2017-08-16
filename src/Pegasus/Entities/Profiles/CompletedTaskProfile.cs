namespace Pegasus.Entities.Profiles
{
    public class CompletedTaskProfile : ITaskProfile
    {
        public string TaskRefStyle => "task-completed-strike";
        public string TaskTextStyle => "task-completed";
        public string TaskTimeStyle => "task-time task-completed";
        public string TaskIcon => "<i class=\"fa fa-check\" aria-hidden=\"true\" data-toggle=\"tooltip\" title=\"Task Completed\"></i>";
    }
}
