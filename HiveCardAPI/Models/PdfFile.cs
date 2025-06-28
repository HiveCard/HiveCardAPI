using System.Text.Json.Serialization;

namespace HiveCardAPI.Models
{
    public class PdfFile
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int StatementId { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public User User { get; set; }
        [JsonIgnore]
        public Statement Statement { get; set; }
    }
}
