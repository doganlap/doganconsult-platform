using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace DoganConsult.UserProfile.UserProfiles;

public interface IUserProfileAppService : ICrudAppService<
    UserProfileDto,
    Guid,
    PagedAndSortedResultRequestDto,
    CreateUpdateUserProfileDto,
    CreateUpdateUserProfileDto>
{
}
