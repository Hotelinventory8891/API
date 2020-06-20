using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Property
{
    public interface IPropertyUserRepository
    {
        IEnumerable<PropertyUserSnapshot> GetFilteredPropertyUserDetails(Expression<Func<PropertyUserSnapshot, bool>> expr1);
        IEnumerable<PropertyUserSnapshot> GetAllPropertyUserDetails();
        long AddPropertyUser(PropertyUserSnapshot property, bool Commit = true);
        bool UpdatePropertyUser(PropertyUserSnapshot property, bool Commit = true);
        bool DeletePropertyUser(PropertyUserSnapshot property, bool Commit = true);
    }
    public interface IPropertyRepository
    {
        IEnumerable<PropertySnapshot> GetFilteredPropertyDetails(Expression<Func<PropertySnapshot, bool>> expr1);
        IEnumerable<PropertySnapshot> GetAllPropertyDetails();
        long AddProperty(PropertySnapshot property, bool Commit = true);
        bool UpdateProperty(PropertySnapshot property, bool Commit = true);
        bool DeleteProperty(PropertySnapshot property, bool Commit = true);
    }
    public interface IPropertyFacilityRepository
    {
        IEnumerable<PropertyFacilitySnapshot> GetFilteredPropertyFacilityDetails(Expression<Func<PropertyFacilitySnapshot, bool>> expr1);
        IEnumerable<PropertyFacilitySnapshot> GetAllPropertyFacilityDetails();
        long AddPropertyFacility(PropertyFacilitySnapshot property, bool Commit = true);
        bool UpdatePropertyFacility(PropertyFacilitySnapshot property, bool Commit = true);
        bool DeletePropertyFacility(PropertyFacilitySnapshot property, bool Commit = true);
    }
    public interface IPropertyDetailsViewRepository
    {
        IEnumerable<PropertyDetailsViewSnapshot> GetFilteredPropertyDetailsViewDetails(Expression<Func<PropertyDetailsViewSnapshot, bool>> expr1);
        IEnumerable<PropertyDetailsViewSnapshot> GetAllPropertyDetailsViewDetails();
    }
}
