using System;
using System.Collections.Generic;
using System.Text;
using DoganConsult.Organization.Localization;
using Volo.Abp.Application.Services;

namespace DoganConsult.Organization;

/* Inherit your application services from this class.
 */
public abstract class OrganizationAppService : ApplicationService
{
    protected OrganizationAppService()
    {
        LocalizationResource = typeof(OrganizationResource);
    }
}
