using Pegasus.Entities.Enumerations;

namespace Pegasus.Entities.Profiles
{
    public class TaskProfileResolver
    {
        public static ITaskProfile SetTaskProfile(TaskStatusEnum taskStatus)
        {
            switch (taskStatus)
            {
                case TaskStatusEnum.Completed:
                    return new CompletedTaskProfile();
                case TaskStatusEnum.Obsolete:
                    return new ObsoleteTaskProfile();
                case TaskStatusEnum.OnHold:
                    return new OnHoldTaskProfile();
                case TaskStatusEnum.InProgress:
                    return new InProgressTaskProfile();
                default:
                    return new DefaultTaskProfile();
            }
        }
    }
}
