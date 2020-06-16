using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public interface IUserRepository
    {
        IEnumerable<UserSnapshot> GetFilteredUserDetails(Expression<Func<UserSnapshot, bool>> expr1);
        IEnumerable<UserSnapshot> GetAllUserDetails();
        bool AddUser(UserSnapshot _User, bool Commit = true);
        bool DeleteUser(UserSnapshot _User, bool Commit = true);
    }
}
