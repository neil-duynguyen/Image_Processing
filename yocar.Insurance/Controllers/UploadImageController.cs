using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
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

/*        [HttpPost("extract-qr-data")]
        public async Task<IActionResult> ExtractQrDataFromQR(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded.");

                var qrData = await _uploadImageServices.ExtractQrDataFromImage(file);

                return Ok(qrData);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }*/

        [HttpPost("extract-image-data")]
        public async Task<IActionResult> ExtractDataFromImage(IFormFileCollection file)
        {
            try
            {
                return Ok(await _uploadImageServices.ProcessImageAsync(file));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
