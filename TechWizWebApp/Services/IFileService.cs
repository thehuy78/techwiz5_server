using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace TechWizWebApp.Services
{
    public interface IFileService
    {
        public Task<string> UploadImageAsync(IFormFile image);
        

    }
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;
        public FileService(IWebHostEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _env = hostingEnvironment;
            _configuration = configuration;
        }

        public async Task<string> UploadImageAsync(IFormFile image)
        {
            // Kiem tra neu folder da duoc tao chua
            var folderImagePath = CreateFolderIfNotExist();

            // Kiem tra phan mo rong theo dieu kien
            var extension = Path.GetExtension(image.FileName).ToLower();
            if (!CheckExtension(extension))
            {
                return @"Invalid file extension (.jpg, .jpeg, .png, .gif).";
            }

            // Tao filename theo Guid de tranh trung lap
            var fileName = GetRandomFilename(extension);
            var filePath = Path.Combine(folderImagePath, fileName);

            // Upload image trong server
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);                
            }


            //Upload image trong firebase(cloud)
            using (var firebaseStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                await UploadImageWithFirebaseAsync(firebaseStream, fileName); // Tải lên Firebase
            }

            return fileName;
        }


        private async Task UploadImageWithFirebaseAsync(FileStream fileStream, string filename)
        {
            string ApiKey = _configuration["FirebaseSettings:ApiKey"];
            string Bucket = _configuration["FirebaseSettings:Bucket"];
            string AuthEmail = _configuration["FirebaseSettings:AuthEmail"];
            string AuthPassword = _configuration["FirebaseSettings:AuthPassword"];

            var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);

            var cancellation = new CancellationTokenSource();

            var task = new FirebaseStorage(
                Bucket, new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel = true
                })
                .Child("Images")
                .Child(filename)
                .PutAsync(fileStream, cancellation.Token);

            try
            {
                string link = await task;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception was thrown: {0}", e.Message);
                throw;
            }            
        }
        
        private string CreateFolderIfNotExist()
        {
            var folderPath = Path.Combine(_env.ContentRootPath, "Images");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            return folderPath;
        }

        private bool CheckExtension(string fileExtension)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

            if (!allowedExtensions.Contains(fileExtension))
                return false;

            return true;
        }

        private string GetRandomFilename(string extension)
        {
            string randomFileName = Guid.NewGuid().ToString() + extension;
            return randomFileName;
        }


    }

}
