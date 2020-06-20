using Common.Logging;
using Core.Property;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Property
{
    public class PropertyRepository: Repository<PropertySnapshot>, IPropertyRepository
    {
        #region Properties

        private ILogger _tracer;

        #endregion Properties


        #region Constructor
        public PropertyRepository(UnitOfWork unitOfWork, ILogger Tracer)
            : base(unitOfWork)
        {
            if (Tracer == null)
                throw new ArgumentNullException("Tracer");
            _tracer = Tracer;
        }

        #endregion

        public IEnumerable<PropertySnapshot> GetFilteredPropertyDetails(Expression<Func<PropertySnapshot, bool>> expr1)
        {
            IEnumerable<PropertySnapshot> result = null;
            try
            {
                result = this.GetFiltered(expr1);
            }
            catch (Exception ex)
            {
                _tracer.LogError("Reading Filtered Property Details", ex);
            }
            return result;
        }
        public IEnumerable<PropertySnapshot> GetAllPropertyDetails()
        {
            IEnumerable<PropertySnapshot> result = null;
            try
            {
                result = this.GetAll();
            }
            catch (Exception ex)
            {
                _tracer.LogError("Reading Property Details", ex);
            }
            return result;
        }
        public long AddProperty(PropertySnapshot _property, bool Commit = true)
        {
            long returnValue = 0;
            try
            {
                this.Add(_property);
                if (Commit)
                    this.UnitOfWork.CommitAndRefreshChanges();

                returnValue = _property.PropertyId;
            }
            catch (Exception ex)
            {
                returnValue = 0;
                _tracer.LogError("Creating Property", ex);
            }
            return returnValue;
        }
        public bool UpdateProperty(PropertySnapshot property, bool Commit = true)
        {
            Expression<Func<PropertySnapshot, bool>> expr1 = null;
            bool returnValue = false;
            try
            {
                expr1 = s => s.PropertyId == property.PropertyId;
                PropertySnapshot origValue = this.GetFiltered(expr1).FirstOrDefault();
                origValue.PropertyName = property.PropertyName;
                origValue.PropertyType = property.PropertyType;
                origValue.No_Of_Rooms = property.No_Of_Rooms;
                origValue.AddressLine1 = property.AddressLine1;
                origValue.AddressLine2 = property.AddressLine2;
                origValue.AddressLine3 = property.AddressLine3;
                origValue.ZipCode = property.ZipCode;
                origValue.Area = property.Area;
                origValue.StateId = property.StateId;
                origValue.CountryId = property.CountryId;
                origValue.Check_In_Time = property.Check_In_Time;
                origValue.Check_Out_Time = property.Check_Out_Time;
                origValue.LastUpdatedBy = property.LastUpdatedBy;
                origValue.LastUpdatedOn = property.LastUpdatedOn;
                this.Modify(origValue);
                if (Commit)
                    this.UnitOfWork.CommitAndRefreshChanges();
                returnValue = true;
            }
            catch (Exception ex)
            {
                returnValue = false;
                _tracer.LogError("Updating Property", ex);
            }
            return returnValue;
        }
        public bool DeleteProperty(PropertySnapshot property, bool Commit = true)
        {
            Expression<Func<PropertySnapshot, bool>> expr1 = null;
            bool returnValue = false;
            try
            {
                expr1 = s => s.PropertyId == property.PropertyId;
                PropertySnapshot origValue = this.GetFiltered(expr1).FirstOrDefault();
                origValue.IsActive = false;
                origValue.LastUpdatedBy = property.LastUpdatedBy;
                origValue.LastUpdatedOn = property.LastUpdatedOn;
                this.Modify(origValue);
                if (Commit)
                    this.UnitOfWork.CommitAndRefreshChanges();
                returnValue = true;
            }
            catch (Exception ex)
            {
                returnValue = false;
                _tracer.LogError("Deleting Property", ex);
            }
            return returnValue;
        }
    }
}
