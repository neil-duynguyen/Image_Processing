using Microsoft.Extensions.Localization;
using yocar.Insurance.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace yocar.Insurance;

[Dependency(ReplaceServices = true)]
public class InsuranceBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<InsuranceResource> _localizer;

    public InsuranceBrandingProvider(IStringLocalizer<InsuranceResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
