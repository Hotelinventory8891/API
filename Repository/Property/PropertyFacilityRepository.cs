using Common.Logging;
using Core.Property;
using DAL;
using Repository.Property;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Property
{
    public class PropertyFacilityRepository : Repository<PropertyFacilitySnapshot>, IPropertyFacilityRepository
    {
        #region Properties

        private ILogger _tracer;

        #endregion Properties

        #region Constructor
        public PropertyFacilityRepository(UnitOfWork unitOfWork, ILogger Tracer)
            : base(unitOfWork)
        {
            if (Tracer == null)
                throw new ArgumentNullException("Tracer");
            _tracer = Tracer;
        }

        #endregion

        public IEnumerable<PropertyFacilitySnapshot> GetFilteredPropertyFacilityDetails(Expression<Func<PropertyFacilitySnapshot, bool>> expr1)
        {
            IEnumerable<PropertyFacilitySnapshot> result = null;
            try
            {
                result = this.GetFiltered(expr1);
            }
            catch (Exception ex)
            {
                _tracer.LogError("Reading Filtered propertyfacility Details", ex);
            }
            return result;
        }
        public IEnumerable<PropertyFacilitySnapshot> GetAllPropertyFacilityDetails()
        {
            IEnumerable<PropertyFacilitySnapshot> result = null;
            try
            {
                result = this.GetAll();
            }
            catch (Exception ex)
            {
                _tracer.LogError("Reading propertyfacility Details", ex);
            }
            return result;
        }
        public long AddPropertyFacility(PropertyFacilitySnapshot _propertyfacility, bool Commit = true)
        {
            long returnValue = 0;
            try
            {
                this.Add(_propertyfacility);
                if (Commit)
                    this.UnitOfWork.CommitAndRefreshChanges();

                returnValue = _propertyfacility.Id;
            }
            catch (Exception ex)
            {
                returnValue = 0;
                _tracer.LogError("Creating propertyfacility", ex);
            }
            return returnValue;
        }
        public bool UpdatePropertyFacility(PropertyFacilitySnapshot propertyfacility, bool Commit = true)
        {
            Expression<Func<PropertyFacilitySnapshot, bool>> expr1 = null;
            bool returnValue = false;
            try
            {
                expr1 = s => s.Id == propertyfacility.Id;
                PropertyFacilitySnapshot origValue = this.GetFiltered(expr1).FirstOrDefault();
                origValue.FacilityId = propertyfacility.FacilityId;
                origValue.PropertyId = propertyfacility.PropertyId;
                origValue.LastUpdatedBy = propertyfacility.LastUpdatedBy;
                origValue.LastUpdatedOn = propertyfacility.LastUpdatedOn;
                this.Modify(origValue);
                if (Commit)
                    this.UnitOfWork.CommitAndRefreshChanges();
                returnValue = true;
            }
            catch (Exception ex)
            {
                returnValue = false;
                _tracer.LogError("Updating propertyfacility", ex);
            }
            return returnValue;
        }
        public bool DeletePropertyFacility(PropertyFacilitySnapshot propertyfacility, bool Commit = true)
        {
            Expression<Func<PropertyFacilitySnapshot, bool>> expr1 = null;
            bool returnValue = false;
            try
            {
                expr1 = s => s.Id == propertyfacility.Id;
                PropertyFacilitySnapshot origValue = this.GetFiltered(expr1).FirstOrDefault();
                origValue.IsActive = false;
                origValue.LastUpdatedBy = propertyfacility.LastUpdatedBy;
                origValue.LastUpdatedOn = propertyfacility.LastUpdatedOn;
                this.Modify(origValue);
                if (Commit)
                    this.UnitOfWork.CommitAndRefreshChanges();
                returnValue = true;
            }
            catch (Exception ex)
            {
                returnValue = false;
                _tracer.LogError("Deleting propertyfacility", ex);
            }
            return returnValue;
        }
    }
}
