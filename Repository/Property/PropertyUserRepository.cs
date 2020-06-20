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
    public class PropertyUserRepository : Repository<PropertyUserSnapshot>, IPropertyUserRepository
    {
        #region Properties

        private ILogger _tracer;

        #endregion Properties


        #region Constructor
        public PropertyUserRepository(UnitOfWork unitOfWork, ILogger Tracer)
            : base(unitOfWork)
        {
            if (Tracer == null)
                throw new ArgumentNullException("Tracer");
            _tracer = Tracer;
        }

        #endregion

        public IEnumerable<PropertyUserSnapshot> GetFilteredPropertyUserDetails(Expression<Func<PropertyUserSnapshot, bool>> expr1)
        {
            IEnumerable<PropertyUserSnapshot> result = null;
            try
            {
                result = this.GetFiltered(expr1);
            }
            catch (Exception ex)
            {
                _tracer.LogError("Reading Filtered PropertyUser Details", ex);
            }
            return result;
        }
        public IEnumerable<PropertyUserSnapshot> GetAllPropertyUserDetails()
        {
            IEnumerable<PropertyUserSnapshot> result = null;
            try
            {
                result = this.GetAll();
            }
            catch (Exception ex)
            {
                _tracer.LogError("Reading PropertyUser Details", ex);
            }
            return result;
        }
        public long AddPropertyUser(PropertyUserSnapshot _PropertyUser, bool Commit = true)
        {
            long returnValue = 0;
            try
            {
                this.Add(_PropertyUser);
                if (Commit)
                    this.UnitOfWork.CommitAndRefreshChanges();

                returnValue = _PropertyUser.Id;
            }
            catch (Exception ex)
            {
                returnValue = 0;
                _tracer.LogError("Creating PropertyUser", ex);
            }
            return returnValue;
        }
        public bool UpdatePropertyUser(PropertyUserSnapshot PropertyUser, bool Commit = true)
        {
            Expression<Func<PropertyUserSnapshot, bool>> expr1 = null;
            bool returnValue = false;
            try
            {
                expr1 = s => s.Id == PropertyUser.Id;
                PropertyUserSnapshot origValue = this.GetFiltered(expr1).FirstOrDefault();
                origValue.OwnerId = PropertyUser.OwnerId;
                origValue.LastUpdatedBy = PropertyUser.LastUpdatedBy;
                origValue.LastUpdatedOn = PropertyUser.LastUpdatedOn;
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
        public bool DeletePropertyUser(PropertyUserSnapshot property, bool Commit = true)
        {
            Expression<Func<PropertyUserSnapshot, bool>> expr1 = null;
            bool returnValue = false;
            try
            {
                expr1 = s => s.PropertyId == property.PropertyId;
                PropertyUserSnapshot origValue = this.GetFiltered(expr1).FirstOrDefault();
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
                _tracer.LogError("Deleting PropertyUser", ex);
            }
            return returnValue;
        }
    }
}
