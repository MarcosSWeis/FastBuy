using Duende.IdentityServer;
using FastBuy.Auth.Api.Contracts;
using FastBuy.Auth.Api.Entity;
using FastBuy.Auth.Api.Mapping;
using FastBuy.Auth.Api.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FastBuy.Auth.Api.Controllers
{
    [ApiController]
    [Route("users")]
    [Authorize(Policy = IdentityServerConstants.LocalApi.PolicyName,Roles = Roles.Admin)]
    public class UsersControllers :ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;

        public UsersControllers(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ApplicationUserDto>> Get()
        {
            var users = userManager.Users.ToList()
                .Select(x => x.MapToApplicationUserDto());

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationUserDto>> GetByIdAsync(Guid id)
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            if (user is null)
                return NotFound();

            return user.MapToApplicationUserDto();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(Guid id,ApplicationUserUpdateDto userDto)
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            if (user is null)
                return NotFound();

            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.DocumentType = (DocumentTypeEnum) userDto.DocumentType;
            user.DocumentNumber = userDto.DocumentNumber;

            await userManager.UpdateAsync(user);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            if (user is null)
                return NotFound();

            await userManager.DeleteAsync(user);
            return NoContent();
        }
    }
}