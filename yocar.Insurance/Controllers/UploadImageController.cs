using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing; // Đảm bảo bạn đã cài đặt System.Drawing.Common
using System.IO;
using System.Threading.Tasks;
using yocar.Insurance.Services;
using ZXing;

namespace yocar.Insurance.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    public class UploadImageController : ControllerBase
    {
        private readonly UploadImageServices _uploadImageServices;

        public UploadImageController(UploadImageServices uploadImageServices)
        {
            _uploadImageServices = uploadImageServices;
        }

      /*  [HttpPost]
        public async Task<IActionResult> UpImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
                return Ok(await _uploadImageServices.UploadImage(file));
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }*/

        [HttpPost("extract-qr-data")]
        public async Task<IActionResult> ExtractQrDataFromImage(IFormFile file)
        {
            try
            {
                return Ok(await _uploadImageServices.ExtractQrDataFromImage(file));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
