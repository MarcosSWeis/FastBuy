using FastBuy.Auth.Api.Contracts;
using FastBuy.Auth.Api.Entity;

namespace FastBuy.Auth.Api.Mapping
{
    public static class MappingExtension
    {

        public static ApplicationUserDto MapToApplicationUserDto(this ApplicationUser user)
        {
            return new ApplicationUserDto()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DocumentType = (short) user.DocumentType,
                DocumentNumber = user.DocumentNumber,
            };
        }
    }
}
