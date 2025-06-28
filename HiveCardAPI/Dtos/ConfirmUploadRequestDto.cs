namespace HiveCardAPI.Dtos
{
    public class ConfirmUploadRequestDto
    {
        public int UserId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string S3Key { get; set; } = string.Empty;
    }
}
