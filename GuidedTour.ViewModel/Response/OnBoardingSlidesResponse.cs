

namespace GuidedTour.ViewModel.Response
{
    public class OnBoardingSlidesResponse
    {
        public int Id { get; set; }
        public int PageId { get; set; }
        public string[] Body { get; set; }
        public string[] Button { get; set; }
        public string Video { get; set; }
        public bool AlreadyKnow { get; set; }
        public bool IsMainPage { get; set; }
    }
}
