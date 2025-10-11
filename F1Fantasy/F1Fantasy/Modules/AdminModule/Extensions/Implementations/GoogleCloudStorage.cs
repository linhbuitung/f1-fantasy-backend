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
        // Check if the file exists
        try
        {
            var existingObject = await storageClient.GetObjectAsync(bucketName, fileNameForStorage);
            if (existingObject != null)
            {
                await storageClient.DeleteObjectAsync(bucketName, fileNameForStorage);
            }
        }
        catch (Google.GoogleApiException ex) when (ex.Error.Code == 404)
        {
            // File does not exist, nothing to delete
        }
        
        using (var memoryStream = new MemoryStream())
        {
            await imageFile.CopyToAsync(memoryStream);
            var dataObject = await storageClient.UploadObjectAsync(bucketName, fileNameForStorage, null, memoryStream);
            return dataObject.MediaLink;
        }
    }

    public async Task DeleteFileAsync(string fileNameForStorage)
    {
        await storageClient.DeleteObjectAsync(bucketName, fileNameForStorage);
    }
}