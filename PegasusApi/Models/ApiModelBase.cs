using System.Collections.Generic;

namespace PegasusApi.Models
{
    public class ApiModelBase
    {
        public bool IsSuccess { get; set; } = true;
        public IEnumerable<string> Errors { get; set; }
    }
}