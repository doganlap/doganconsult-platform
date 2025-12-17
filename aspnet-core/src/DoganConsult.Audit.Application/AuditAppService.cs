using System;
using System.Collections.Generic;
using System.Text;
using DoganConsult.Audit.Localization;
using Volo.Abp.Application.Services;

namespace DoganConsult.Audit;

/* Inherit your application services from this class.
 */
public abstract class AuditAppService : ApplicationService
{
    protected AuditAppService()
    {
        LocalizationResource = typeof(AuditResource);
    }
}
