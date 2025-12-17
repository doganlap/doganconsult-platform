using System;
using System.Collections.Generic;
using System.Text;
using DoganConsult.Document.Localization;
using Volo.Abp.Application.Services;

namespace DoganConsult.Document;

/* Inherit your application services from this class.
 */
public abstract class DocumentAppService : ApplicationService
{
    protected DocumentAppService()
    {
        LocalizationResource = typeof(DocumentResource);
    }
}
