namespace OnlineOrderingSystem.Models
{
    public class ChatMessage
    {
        public int ChatMessageId { get; set; }
        public string Message { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
        public DateTime SendAt { get; set; }

        public int ConversationId { get; set; }
        public virtual Conversation Conversation { get; set; }
    }
}
