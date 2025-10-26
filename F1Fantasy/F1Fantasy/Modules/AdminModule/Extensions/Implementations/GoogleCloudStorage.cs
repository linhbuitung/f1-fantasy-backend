using F1Fantasy.Modules.AdminModule.Extensions.Interfaces;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;

namespace F1Fantasy.Modules.AdminModule.Extensions.Implementations;

public class GoogleCloudStorage : ICloudStorage
{
    private readonly GoogleCredential googleCredential;
    private readonly StorageClient storageClient;
    private readonly string bucketName;

    public GoogleCloudStorage(IConfiguration configuration)
    {
        // Get base64 string from environment variable
        var base64Credential = Environment.GetEnvironmentVariable("GOOGLE_CREDENTIAL_BASE64");
        if (string.IsNullOrEmpty(base64Credential))
            throw new InvalidOperationException("GOOGLE_CREDENTIAL_BASE64 environment variable is not set.");

        // Decode base64 to byte array
        var credentialBytes = Convert.FromBase64String(base64Credential);

        // Create credential from stream
        using var credentialStream = new MemoryStream(credentialBytes);
        googleCredential = GoogleCredential.FromStream(credentialStream);
        
        storageClient = StorageClient.Create(googleCredential);
        bucketName = configuration.GetValue<string>("AdminSettings:GoogleCloudStorageBucket");
    }

    public async Task<string> UploadFileAsync(IFormFile imageFile, string fileNameForStorage)
    {
        using (var memoryStream = new MemoryStream())
        {
            await imageFile.CopyToAsync(memoryStream);
            
            memoryStream.Position = 0;

            var contentType = string.IsNullOrWhiteSpace(imageFile.ContentType)
                ? GetContentTypeFromFileName(fileNameForStorage)
                : imageFile.ContentType;
            
            // Create object with Cache-Control set so caches respect your desired TTL
            var obj = new Google.Apis.Storage.v1.Data.Object
            {
                Bucket = bucketName,
                Name = fileNameForStorage,
                ContentType = contentType,
                CacheControl = "no-cache, max-age=0, must-revalidate"
            };
            
            var uploaded = await storageClient.UploadObjectAsync(obj, memoryStream);
            var versionedUrl = $"https://storage.googleapis.com/{bucketName}/{Uri.EscapeDataString(fileNameForStorage)}?generation={uploaded.Generation}";
            return versionedUrl;
        }
    }

    public async Task DeleteFileAsync(string fileNameForStorage)
    {
        await storageClient.DeleteObjectAsync(bucketName, fileNameForStorage);
    }
    
    private string GetContentTypeFromFileName(string fileName)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        return ext switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            _ => "application/octet-stream"
        };
    }
}