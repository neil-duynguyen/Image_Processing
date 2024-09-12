
using Volo.Abp.Application.Services;
using System.Drawing;
using ZXing;
using OpenCvSharp.Text;
using OpenCvSharp;
using Mat = OpenCvSharp.Mat;
using QRCodeDetector = OpenCvSharp.QRCodeDetector;
using OutputArray = OpenCvSharp.OutputArray;


namespace yocar.Insurance.Services
{
    public class UploadImageServices : ApplicationService
    {
        private const string TesseractDataPath = @"./tessdata";
        public static class BitmapExtension;
        public async Task<object> ExtractQrDataFromImage(IFormFile file)
        {
            if (file == null || file.Length == 0) throw new Exception("No file uploaded.");

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                var imageBytes = memoryStream.ToArray();

                Mat processedImage = ProcessImage(imageBytes);

                // Convert the processed image to byte array
                using (var processedMemoryStream = new MemoryStream())
                {
                    Cv2.ImEncode(".png", processedImage, out var processedImageBytes);
                    processedMemoryStream.Write(processedImageBytes, 0, processedImageBytes.Length);
                    processedMemoryStream.Seek(0, SeekOrigin.Begin);
 
                    return ExtractQrCodeData(processedMemoryStream.ToArray());
                }
            }
        }

        // Extract QR code data from the processed image
        private object ExtractQrCodeData(byte[] imageBytes)
        {
            using (var ms = new MemoryStream(imageBytes))
            {
                using (var bitmap = new Bitmap(ms))
                {
                    var reader = new BarcodeReader();
                    var result = reader.Decode(bitmap);

                    if (result == null)
                        throw new Exception("QR code could not be decoded.");

                    var dataParts = result.Text.Split('|');

                    if (dataParts.Length < 7)
                        throw new Exception("Invalid QR code format.");

                    return new
                    {
                        IdNumberCccd = dataParts[0],
                        IdNumberCmnd = dataParts[1],
                        Name = dataParts[2],
                        DateOfBirth = $"{dataParts[3].Substring(0, 2)}/{dataParts[3].Substring(2, 2)}/{dataParts[3].Substring(4, 4)}",
                        Gender = dataParts[4],
                        Address = dataParts[5],
                        DateOfIssue = $"{dataParts[6].Substring(0, 2)}/{dataParts[6].Substring(2, 2)}/{dataParts[6].Substring(4, 4)}"
                    };
                }
            }
        }

        public async Task<string> ExtractTextWithOCRTesseract(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);

                var imageBytes = memoryStream.ToArray();

                var denoised = ProcessImage(imageBytes);

                using (var ocr = OCRTesseract.Create(TesseractDataPath, "vie", null, 3, 3))
                {
                    // Prepare output variables
                    string text;
                    OpenCvSharp.Rect[] boxes = null;
                    string[] words = null;
                    float[] confidences = null;

                    ocr.Run(denoised, out text, out boxes, out words, out confidences, ComponentLevels.Word);
                    return text;
                }

            }
        }



        public Mat ProcessImage(byte[] imageBytes)
        {
            Mat img = Cv2.ImDecode(imageBytes, ImreadModes.Color);

            if (img.Empty())
            {
                throw new InvalidOperationException("Failed to load image.");
            }

            Mat processedImage = new Mat();

            Cv2.FastNlMeansDenoisingColored(img, processedImage, 10, 10, 7, 21);

            return processedImage;
        }


    }
}
