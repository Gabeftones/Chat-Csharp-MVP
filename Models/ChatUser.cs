namespace ChatMVC.Models
{
    public class ChatUser
    {
        public string UserName { get; set; } = string.Empty;

        public string Nome { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Situacao { get; set; } = "Ativo";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
