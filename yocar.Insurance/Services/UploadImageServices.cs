
using Volo.Abp.Application.Services;
using System.Drawing;
using ZXing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static yocar.Insurance.Entities.DataCCCDEntity;


namespace yocar.Insurance.Services
{
    public class UploadImageServices : ApplicationService
    {
        private const string TesseractDataPath = @"./tessdata";

        private static readonly HttpClient _httpClient = new HttpClient();

        private const string ApiUrl = "https://api.fpt.ai/vision/idr/vnm";
        private const string ApiKey = "L5Q03m0Tga9w37XlgNzA0k726T3oDIXH";

        // Extract QR code data from the processed image
        private DataCCCD ExtractQrCodeData(byte[] imageBytes)
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

                    return new DataCCCD
                    {
                        IdNumber = dataParts[0],
                        Name = dataParts[2],
                        DateOfBirth = $"{dataParts[3].Substring(0, 2)}/{dataParts[3].Substring(2, 2)}/{dataParts[3].Substring(4, 4)}",
                        Gender = dataParts[4],
                        Address = dataParts[5],
                        DateOfIssue = $"{dataParts[6].Substring(0, 2)}/{dataParts[6].Substring(2, 2)}/{dataParts[6].Substring(4, 4)}"
                    };
                }
            }
        }

/*        public async Task<string> ExtractTextWithOCRTesseract(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);

                var imageBytes = memoryStream.ToArray();

                using (var img = Mat.FromImageData(imageBytes))
                {
                    using (var ocr = OCRTesseract.Create(TesseractDataPath, "vie", null, 3, 3))
                    {
                        // Prepare output variables
                        string text;
                        OpenCvSharp.Rect[] boxes = null;
                        string[] words = null;
                        float[] confidences = null;

                        ocr.Run(img, out text, out boxes, out words, out confidences, ComponentLevels.Word);
                        return text;
                    }
                }
            }
        }*/

        public async Task<DataCCCD> ExtractQrDataFromImage(IFormFile file)
        {
            if (file == null || file.Length == 0) throw new Exception("No file uploaded.");

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                var imageBytes = memoryStream.ToArray();
                return ExtractQrCodeData(imageBytes);
            }
        }

        public async Task<object> ProcessImageAsync(IFormFileCollection files)
        {
            var data = new DataCCCD();
            var dataFromQR = new DataCCCD();
            foreach (var file in files)
            {
                using var content = new MultipartFormDataContent();
                using var fileStream = file.OpenReadStream();
                var streamContent = new StreamContent(fileStream)
                {
                    Headers = { ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png") }
                };
                content.Add(streamContent, "image", file.FileName);

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("api-key", ApiKey);

                var response = await _httpClient.PostAsync(ApiUrl, content);
                response.EnsureSuccessStatusCode();

                var jsonResult = await response.Content.ReadAsStringAsync();
                var jsonObject = JObject.Parse(jsonResult);
                var dataArray = jsonObject["data"] as JArray;

                if (dataArray == null) throw new Exception("Error Process Image.");

                var dataItem = dataArray.FirstOrDefault(item => item["id"] != null);

                if (dataItem != null)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponseFront>(jsonResult);
                    var frontData = apiResponse?.Data?.FirstOrDefault();
                    if (frontData != null)
                    {
                        data.IdNumber = frontData.Id;
                        data.Name = frontData.Name;
                        data.DateOfBirth = frontData.Dob;
                        data.Gender = frontData.Sex;
                        data.Address = frontData.Address;
                    }

                    dataFromQR = (DataCCCD)await ExtractQrDataFromImage(file);
                }
                else
                {
                    var apiResponseBackside = JsonConvert.DeserializeObject<ApiResponseBackside>(jsonResult);
                    var backsideData = apiResponseBackside?.Data?.FirstOrDefault();
                    if (backsideData != null)
                    {
                        data.DateOfIssue = backsideData.Issue_Date;
                    }
                }
            }
            return CompareDataCCCDs(data, dataFromQR);
        }
    }
}
