using System;
using System.Collections.Generic;
using System.Text;
using DoganConsult.AI.Localization;
using Volo.Abp.Application.Services;

namespace DoganConsult.AI;

/* Inherit your application services from this class.
 */
public abstract class AIAppService : ApplicationService
{
    protected AIAppService()
    {
        LocalizationResource = typeof(AIResource);
    }
}
