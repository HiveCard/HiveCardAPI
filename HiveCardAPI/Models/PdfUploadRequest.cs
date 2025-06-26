namespace HiveCardAPI.Models
{
    public class PdfUploadRequest
    {
        public int UserId { get; set; }
        public string FileName { get; set; } = "";
    }
}
