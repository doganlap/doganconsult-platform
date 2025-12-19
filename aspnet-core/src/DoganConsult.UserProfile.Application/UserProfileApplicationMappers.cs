using System.Collections.Generic;
using Riok.Mapperly.Abstractions;
using DoganConsult.UserProfile.UserProfiles;

namespace DoganConsult.UserProfile;

[Mapper]
public partial class UserProfileApplicationMappers
{
    [MapProperty(nameof(UserProfiles.UserProfile.SystemRole), nameof(UserProfileDto.SystemRole))]
    [MapProperty(nameof(UserProfiles.UserProfile.StakeholderType), nameof(UserProfileDto.StakeholderType))]
    [MapperIgnoreSource(nameof(UserProfiles.UserProfile.ExtraProperties))]
    [MapperIgnoreSource(nameof(UserProfiles.UserProfile.ConcurrencyStamp))]
    public partial UserProfileDto ToUserProfileDto(UserProfiles.UserProfile source);
    
    public partial List<UserProfileDto> ToUserProfileDtoList(List<UserProfiles.UserProfile> source);
}
