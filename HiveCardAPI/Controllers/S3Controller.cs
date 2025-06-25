using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using Microsoft.AspNetCore.Mvc;


namespace HiveCardAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class S3Controller : ControllerBase
    {
        private readonly IConfiguration _config;

        public S3Controller(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("presign")]
        public IActionResult GetPreSignedUrl([FromQuery] string fileName, [FromQuery] int userId)
        {
            var bucket = _config["S3:Bucket"];
            var region = RegionEndpoint.GetBySystemName(_config["S3:Region"]);
            var key = $"uploads/{userId}/{fileName}";

            var credentials = new BasicAWSCredentials(
                _config["S3:AccessKey"],
                _config["S3:SecretKey"]);

            using var client = new AmazonS3Client(credentials, region);

            var request = new GetPreSignedUrlRequest
            {
                BucketName = bucket,
                Key = key,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddMinutes(15),
                ContentType = "application/pdf"
            };

            var url = client.GetPreSignedURL(request);

            return Ok(new
            {
                uploadUrl = url,
                fileKey = key
            });
        }
    }
}
