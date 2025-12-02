using Pharmacy_API.Models;

namespace Schematics.API.Service.Infrastructure;

public interface IJwtTokenService
{
    string CreateToken(ApplicationUser user, IList<string> roles);

}
