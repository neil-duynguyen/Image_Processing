using Emgu.CV.Structure;
using Emgu.CV;
using Volo.Abp.Application.Services;
using System.Drawing;
using System.Drawing.Imaging;
using ZXing;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace yocar.Insurance.Services
{
    public class UploadImageServices : ApplicationService
    {
        public async Task<object> ExtractQrDataFromImage(IFormFile file)
        {
            if (file == null || file.Length == 0) throw new Exception("No file uploaded.");

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                var imageBytes = memoryStream.ToArray();

                var qrData = ExtractQrCodeData(imageBytes);

                return qrData;
            }

        }

        private object ExtractQrCodeData(byte[] imageBytes)
        {
            using (var ms = new MemoryStream(imageBytes))
            {
                using (var bitmap = new Bitmap(ms))
                {
                    var reader = new BarcodeReader();

                    var result = reader.Decode(bitmap);
                    var dataParts = result.Text.Split('|');

                    string dateOfBirth = $"{dataParts[3].Substring(0, 2)}/{dataParts[3].Substring(2, 2)}/{dataParts[3].Substring(4, 4)}";
                    string dateOfIssue = $"{dataParts[6].Substring(0, 2)}/{dataParts[6].Substring(2, 2)}/{dataParts[6].Substring(4, 4)}";

                    return new
                    {
                        IdNumberCccd = dataParts[0],
                        IdNumberCmnd = dataParts[1],
                        Name = dataParts[2],
                        DateOfBrith = dateOfBirth,
                        Gender = dataParts[4],
                        Address = dataParts[5],
                        DateOfIssue = dateOfIssue
                    };
                }
            }
        }




    }
}
