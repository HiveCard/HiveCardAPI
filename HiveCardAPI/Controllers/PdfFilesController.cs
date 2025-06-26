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

        //[HttpPost("register-upload")]
        //public async Task<IActionResult> RegisterUpload([FromBody] PdfUploadRequest request)
        //{
        //    var bucket = _config["S3:Bucket"];
        //    var region = RegionEndpoint.GetBySystemName(_config["S3:Region"]);
        //    var sanitizedFileName = Path.GetFileName(request.FileName);
        //    var key = $"uploads/{request.UserId}/{sanitizedFileName}";
        //    //var key = $"uploads/{request.UserId}/{request.FileName}";

        //    var credentials = new BasicAWSCredentials(
        //        _config["S3:AccessKey"],
        //        _config["S3:SecretKey"]);

        //    using var client = new AmazonS3Client(credentials, region);

        //    var presignRequest = new GetPreSignedUrlRequest
        //    {
        //        BucketName = bucket,
        //        Key = key,
        //        Verb = HttpVerb.PUT,
        //        Expires = DateTime.UtcNow.AddMinutes(15),
        //        ContentType = "application/pdf"
        //    };

        //    var uploadUrl = client.GetPreSignedURL(presignRequest);

        //    var file = new PdfFile
        //    {
        //        UserId = request.UserId,
        //        FileName = request.FileName,
        //        FileUrl = $"https://{bucket}.s3.amazonaws.com/{key}",
        //        UploadedAt = DateTime.UtcNow
        //    };

        //    _db.PdfFiles.Add(file);
        //    await _db.SaveChangesAsync();

        //    return Ok(new
        //    {
        //        uploadUrl,
        //        fileId = file.Id,
        //        fileUrl = file.FileUrl
        //    });
        //}

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

            // ✅ Upload the file from local path to S3 using the pre-signed URL
            if (!System.IO.File.Exists(request.FileName))
                return BadRequest("Source PDF file not found.");

            var fileBytes = await System.IO.File.ReadAllBytesAsync(request.FileName);

            using var http = new HttpClient();
            using var putRequest = new HttpRequestMessage(HttpMethod.Put, uploadUrl)
            {
                Content = new ByteArrayContent(fileBytes)
            };
            putRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

            var response = await http.SendAsync(putRequest);
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, $"S3 upload failed: {await response.Content.ReadAsStringAsync()}");
            }

            // ✅ Save to DB after successful upload
            var file = new PdfFile
            {
                UserId = request.UserId,
                FileName = sanitizedFileName,
                FileUrl = $"https://{bucket}.s3.amazonaws.com/{key}",
                UploadedAt = DateTime.UtcNow
            };

            _db.PdfFiles.Add(file);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                uploadUrl,
                fileId = file.Id,
                fileUrl = file.FileUrl
            });
        }

        [HttpPost]
        public async Task<IActionResult> UploadPdf([FromBody] PdfFileUploadDto dto)
        {
            var pdfFile = new PdfFile
            {
                UserId = dto.UserId,
                FileName = dto.FileName,
                FileUrl = dto.FileUrl,
                UploadedAt = DateTime.UtcNow,
                StatementId = 0 // Set later after parsing
            };

            _db.PdfFiles.Add(pdfFile);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = pdfFile.Id }, pdfFile);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var file = await _db.PdfFiles
                .Include(f => f.User)
                .Include(f => f.Statement)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (file == null) return NotFound();
            return Ok(file);
        }
    }


}
