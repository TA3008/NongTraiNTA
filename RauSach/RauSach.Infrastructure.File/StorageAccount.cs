using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using RauSach.Application.Infrastructures;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;

namespace RauSach.Infrastructure.File
{
    public class StorageAccount : IFileService
    {
        private readonly IConfiguration _configuration;
        private Lazy<string> _connectionString;
        public StorageAccount(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = new Lazy<string>(() => _configuration.GetSection("AzureStorage").Value);
        }

        public string Upsert(string rootFolder, string filePath, string base64FileContent)
        {
            BlobContainerClient container = new BlobContainerClient(_connectionString.Value, rootFolder.ToLower());

            if (!container.Exists())
            {
                container.Create();
                container.SetAccessPolicy(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
            }

            byte[] bytes = Convert.FromBase64String(base64FileContent);

            using (MemoryStream ms = new MemoryStream(bytes))
            {
                var blobClient = container.GetBlobClient(filePath);
                blobClient.Upload(ms, true);
                return blobClient.Uri.AbsoluteUri;
            }
        }

        public string UpsertImage(string rootFolder, string filePath, Stream fileStream)
        {
            BlobContainerClient container = new BlobContainerClient(_connectionString.Value, rootFolder.ToLower());

            if (!container.Exists())
            {
                container.Create();
                container.SetAccessPolicy(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
            }
            var blobClient = container.GetBlobClient(filePath);
            var imageStream = SaveJpeg(fileStream, 30);
            imageStream.Position = 0;
            blobClient.Upload(imageStream, true);
            return blobClient.Uri.AbsoluteUri;
        }

        public void Delete(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;
            path = path.Replace("https://", null).Replace("http://", null);
            var containerName = path.Split('/')[1];
            var blobName = path.Replace(path.Split('/')[0] + "/" + containerName, null);

            BlobContainerClient container = new BlobContainerClient(_connectionString.Value, containerName);
            if (container.Exists())
            {
                var blobClient = container.GetBlobClient(blobName);
                if (blobClient.Exists())
                    blobClient.Delete();
            }
        }

        /// <summary> 
        /// Saves an image as a jpeg image, with the given quality 
        /// </summary> 
        /// <param name="path"> Path to which the image would be saved. </param> 
        /// <param name="quality"> An integer from 0 to 100, with 100 being the highest quality. </param> 
        public static Stream SaveJpeg(Stream stream, int quality)
        {
            if (quality < 0 || quality > 100)
                throw new ArgumentOutOfRangeException("quality must be between 0 and 100.");

            var image = Image.FromStream(stream);
            // Encoder parameter for image quality 
            EncoderParameter qualityParam = new EncoderParameter(Encoder.Quality, quality);
            // JPEG image codec 
            ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg");
            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;
            var outsteam = new MemoryStream();
            image.Save(outsteam, jpegCodec, encoderParams);

            return outsteam;
        }

        public string ResizeImageJpeg(Stream stream, int w, int h, string rootFolder, string filePath)
        {
            using (Image img = Image.FromStream(stream))
            {
                using (Bitmap b = new Bitmap(img, new Size(w, h)))
                {
                    using (MemoryStream ms2 = new MemoryStream())
                    {
                        b.Save(ms2, System.Drawing.Imaging.ImageFormat.Jpeg);
                        return UpsertImage(rootFolder, filePath, ms2);
                    }
                }
            }
        }

        /// <summary> 
        /// Returns the image codec with the given mime type 
        /// </summary> 
        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            // Get image codecs for all image formats 
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            // Find the correct image codec 
            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];

            return null;
        }

        public void CreateIfMissing(string path)
        {
            bool folderExists = Directory.Exists(path);
            if (!folderExists)
                Directory.CreateDirectory(path);
        }

        public async Task<string> UploadFile(Microsoft.AspNetCore.Http.IFormFile file, string sDirectory, string newname = null)
        {
            try
            {
                if (newname == null) newname = file.FileName;
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", sDirectory);
                CreateIfMissing(path);
                string pathFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", sDirectory, newname);
                var supportedTypes = new[] { "jpg", "jpeg", "png", "gif", "webp" };
                var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);
                if (!supportedTypes.Contains(fileExt.ToLower())) /// Khác các file định nghĩa
                {
                    return null;
                }
                else
                {
                    using (var stream = new FileStream(pathFile, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    return newname;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}