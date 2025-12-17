using DoganConsult.UserProfile.Localization;
using DoganConsult.UserProfile.UserProfiles;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace DoganConsult.UserProfile.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class UserProfileController : AbpControllerBase
{
    protected UserProfileController()
    {
        LocalizationResource = typeof(UserProfileResource);
    }
}

[Route("api/userprofile/userprofiles")]
public class UserProfileApiController : UserProfileController
{
    private readonly IUserProfileAppService _userProfileAppService;

    public UserProfileApiController(IUserProfileAppService userProfileAppService)
    {
        _userProfileAppService = userProfileAppService;
    }

    [HttpGet]
    public Task<PagedResultDto<UserProfileDto>> GetListAsync([FromQuery] PagedAndSortedResultRequestDto input)
    {
        return _userProfileAppService.GetListAsync(input);
    }

    [HttpGet("{id}")]
    public Task<UserProfileDto> GetAsync(Guid id)
    {
        return _userProfileAppService.GetAsync(id);
    }

    [HttpPost]
    public Task<UserProfileDto> CreateAsync([FromBody] CreateUpdateUserProfileDto input)
    {
        return _userProfileAppService.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public Task<UserProfileDto> UpdateAsync(Guid id, [FromBody] CreateUpdateUserProfileDto input)
    {
        return _userProfileAppService.UpdateAsync(id, input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _userProfileAppService.DeleteAsync(id);
    }
}
