namespace Pegasus.Entities.Profiles
{
    public class InProgressTaskProfile : ITaskProfile
    {
        public string TaskRefStyle => "";
        public string TaskTextStyle => "task-in-progress";
        public string TaskTimeStyle => "task-time";
        public string TaskIcon => "<i class=\"fa fa-signal\" aria-hidden=\"true\" data-toggle=\"tooltip\" title=\"Task In Progress\"></i>";
    }
}
