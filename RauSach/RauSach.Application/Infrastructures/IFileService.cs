namespace RauSach.Application.Infrastructures
{
    public interface IFileService
    {
        /// <summary>
        /// Example Upsert("test", "2021/11/abc.png", "a base64 string")
        /// </summary>
        /// <param name="rootFolder"></param>
        /// <param name="filePath"></param>
        /// <param name="base64FileContent"></param>
        /// <returns></returns>
        string Upsert(string rootFolder, string filePath, string base64FileContent);

        /// <summary>
        /// Example Upsert("test", "2021/11/abc.png", stream)
        /// </summary>
        /// <param name="rootFolder"></param>
        /// <param name="filePath"></param>
        /// <param name="fileStream"></param>
        /// <returns></returns>
        string UpsertImage(string rootFolder, string filePath, Stream fileStream);

        /// <summary>
        /// resize image
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="rootFolder"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        string ResizeImageJpeg(Stream stream, int w, int h, string rootFolder, string filePath);

        /// <summary>
        /// Example Delete("https://abc.blob.net/2021/11/abc.png")
        /// </summary>
        /// <param name="path"></param>
        void Delete(string path);

        void CreateIfMissing(string path);

        Task<string> UploadFile(Microsoft.AspNetCore.Http.IFormFile file, string sDirectory, string newname = null);
    }
}
