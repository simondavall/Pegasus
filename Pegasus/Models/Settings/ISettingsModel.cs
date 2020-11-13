namespace Pegasus.Models.Settings
{
    public interface ISettingsModel
    {
        bool PaginationEnabled { get; set; }
        int PageSize { get; set; }
    }
}