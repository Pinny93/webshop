namespace WebApp.Misc
{
    public class PageMessage
    {
        public PageMessage()
        {
        }

        public PageMessage(string message, MessageType type)
        {
            Message = message;
            Type = type;
        }

        public string Message { get; set; } = "(Message)";

        public MessageType Type { get; set; }
    }

    public enum MessageType
    {
        Light,
        Dark,
        Info,
        Secondary,
        Primary,
        Success,
        Warning,
        Danger,
    }
}