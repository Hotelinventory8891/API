using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Lookup
{
    public interface ILookupRepository
    {
        IEnumerable<LookupSnapshot> GetFilteredLookupDetails(Expression<Func<LookupSnapshot, bool>> expr1);
        IEnumerable<LookupSnapshot> GetAllLookupDetails();
    }
    public interface ILookupTypeRepository
    {
        IEnumerable<LookupTypeSnapshot> GetFilteredLookupTypeDetails(Expression<Func<LookupTypeSnapshot, bool>> expr1);
        IEnumerable<LookupTypeSnapshot> GetAllLookupTypeDetails();
    }
    public interface ILookupViewRepository
    {
        IEnumerable<LookUpViewSnapshot> GetFilteredLookupViewDetails(Expression<Func<LookUpViewSnapshot, bool>> expr1);
        IEnumerable<LookUpViewSnapshot> GetAllLookupViewDetails();
    }
}
