

namespace GuidedTour.Common
{
    public enum MessageType
    {
        /// <summary>
        /// The information
        /// </summary>
        Info,
        /// <summary>
        /// The success
        /// </summary>
        Success,
        /// <summary>
        /// The alert
        /// </summary>
        Alert,
        /// <summary>
        /// The warning
        /// </summary>
        Warning,
        /// <summary>
        /// The error
        /// </summary>
        Error,
    }

    public enum DemoBookingProcess
    {
        EmailVerificationPending = 0,
        EmailVerifed = 1,
    }

    public enum TemplateCodes
    {
        VE = 1 //Verification Email
    }

    public enum ActionType
    {
        Skip=1,
        Ready
    }

    public enum ControlType
    {
        Body=1,
        Button,
        Video
    }
}
