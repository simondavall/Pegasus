using Pegasus.Models.Settings;

namespace Pegasus.Models
{
    public class PaginationViewModel
    {
        private readonly ISettingsModel _settingsModel;

        public PaginationViewModel(ISettingsModel settingsModel)
        {
            _settingsModel = settingsModel;
        }

        public string Action { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }

        public int NextPage
        {
            get { return CurrentPage < TotalPages ? CurrentPage + 1 : 0; }
        }

        public int PrevPage
        {
            get { return CurrentPage > 1 ? CurrentPage - 1 : 0; }
        }

        public bool ShowPagination
        {
            get { return _settingsModel.PaginationEnabled && TotalPages > 1; }
        }

        public bool ShowPrevPage
        {
            get { return PrevPage > 0; }
        }

        public bool ShowNextPage
        {
            get { return NextPage > 0; }
        }
    }
}
