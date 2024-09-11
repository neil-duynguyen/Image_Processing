using yocar.Insurance.Localization;
using Volo.Abp.Application.Services;

namespace yocar.Insurance.Services;

/* Inherit your application services from this class. */
public abstract class InsuranceAppService : ApplicationService
{
    protected InsuranceAppService()
    {
        LocalizationResource = typeof(InsuranceResource);
    }
}