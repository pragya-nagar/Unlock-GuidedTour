
using System;

namespace GuidedTour.EF.Entity
{
    public partial class OnBoardingControl
    {
        public int Id { get; set; }
        public long EmployeeId { get; set; }
        public int SkipCount { get; set; }
        public int ReadyCount { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public long? UpdatedBy { get; set; } = null;
        public DateTime? UpdatedOn { get; set; } = null;
        public bool IsActive { get; set; } = true;

    }
}
