using Meritocious.Core.Extensions;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Entities
{
    public class Role : IdentityRole<UlidId<User>>
    {
    }

    public class UserRole : IdentityUserRole<UlidId<User>>
    {
    }

    public class UserClaim : IdentityUserClaim<UlidId<User>>
    {
    }

    public class UserLogin : IdentityUserLogin<UlidId<User>>
    {
    }

    public class UserToken : IdentityUserToken<UlidId<User>>
    {
    }

    public class RoleClaim : IdentityRoleClaim<UlidId<User>>
    {
    }
}
