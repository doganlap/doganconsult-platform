using System;
using System.Collections.Generic;
using System.Text;
using DoganConsult.Workspace.Localization;
using Volo.Abp.Application.Services;

namespace DoganConsult.Workspace;

/* Inherit your application services from this class.
 */
public abstract class WorkspaceAppService : ApplicationService
{
    protected WorkspaceAppService()
    {
        LocalizationResource = typeof(WorkspaceResource);
    }
}
