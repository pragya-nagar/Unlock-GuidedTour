using System;


namespace GuidedTour.EF.Entity
{
    public partial class OnBoardingScreen
    {
        public int ScreenId { get; set; }
        public int PageId { get; set; }
        public string PageName { get; set; }
        public string ControlType { get; set; }
        public string ControlValue { get; set; }
        public int Flow { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public long? UpdatedBy { get; set; } = null;
        public DateTime? UpdatedOn { get; set; } = null;
        public bool IsActive { get; set; } = true;
    }
}
