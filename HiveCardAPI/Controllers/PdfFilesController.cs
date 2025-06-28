using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using HiveCardAPI.Data;
using HiveCardAPI.Dtos;
using HiveCardAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;

namespace HiveCardAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PdfFilesController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _db;

        public PdfFilesController(IConfiguration config, AppDbContext db)
        {
            _config = config;
            _db = db;
        }

      

        [HttpPost("register-upload")]
        public async Task<IActionResult> RegisterUpload([FromBody] PdfUploadRequest request)
        {
            var bucket = _config["S3:Bucket"];
            var region = RegionEndpoint.GetBySystemName(_config["S3:Region"]);
            var sanitizedFileName = Path.GetFileName(request.FileName);
            var key = $"uploads/{request.UserId}/{sanitizedFileName}";

            var credentials = new BasicAWSCredentials(
                _config["S3:AccessKey"],
                _config["S3:SecretKey"]);

            using var s3Client = new AmazonS3Client(credentials, region);

            var presignRequest = new GetPreSignedUrlRequest
            {
                BucketName = bucket,
                Key = key,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddMinutes(15),
                ContentType = "application/pdf"
            };

            var uploadUrl = s3Client.GetPreSignedURL(presignRequest);

            return Ok(new
            {
                uploadUrl,
                s3Key = key,
                fileName = sanitizedFileName
            });
        }

        [HttpPost("confirm-upload")]
        public async Task<IActionResult> ConfirmUpload([FromBody] ConfirmUploadRequestDto request)
        {
            var bucket = _config["S3:Bucket"];

            var pdfFile = new PdfFile
            {
                UserId = request.UserId,
                FileName = request.FileName,
                FileUrl = $"https://{bucket}.s3.amazonaws.com/{request.S3Key}",
                UploadedAt = DateTime.UtcNow
            };

            _db.PdfFiles.Add(pdfFile);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                fileId = pdfFile.Id,
                fileUrl = pdfFile.FileUrl
            });
        }
    }


}
