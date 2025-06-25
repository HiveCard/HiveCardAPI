namespace HiveCardAPI.Dtos
{
    public class PdfFileUploadDto
    {
        public int UserId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
    }
}
