using Meritocious.Core.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Entities
{
    public class Role : IdentityRole<UlidId<User>>
    {
        private const int UlidLength = 26;
        private UlidId<User> ulidId = UlidId<User>.New();

        [BackingField(nameof(ulidId))]
        public override UlidId<User> Id
        {
            get
            {
                return ulidId;
            }
            set
            {
                if (value == default || string.IsNullOrWhiteSpace(value.Value))
                {
                    ulidId = UlidId<User>.New();
                    return;
                }

                var ulid = value.Value.Length > UlidLength
                    ? value.Value.Substring(0, UlidLength)
                    : value.Value;

                ulidId = new UlidId<User>(ulid);
            }
        }
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
