namespace SecureFileShare.Data.DataTransferObjects
{
    public class ChatDTO
    {

        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string Content { get; set; }
        public string? FileURL { get; set; } = null;
        public string Timestamp { get; set; }
        public int messageId { get; set; }
        public bool isRead { get; set; }

    }
}
