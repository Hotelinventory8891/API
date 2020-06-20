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
    public class RoomFacilityRepository : Repository<RoomFacilitySnapshot>, IRoomFacilityRepository
    {
        #region Properties

        private ILogger _tracer;

        #endregion Properties

        #region Constructor
        public RoomFacilityRepository(UnitOfWork unitOfWork, ILogger Tracer)
            : base(unitOfWork)
        {
            if (Tracer == null)
                throw new ArgumentNullException("Tracer");
            _tracer = Tracer;
        }

        #endregion

        public IEnumerable<RoomFacilitySnapshot> GetFilteredRoomFacilityDetails(Expression<Func<RoomFacilitySnapshot, bool>> expr1)
        {
            IEnumerable<RoomFacilitySnapshot> result = null;
            try
            {
                result = this.GetFiltered(expr1);
            }
            catch (Exception ex)
            {
                _tracer.LogError("Reading Filtered Roomfacility Details", ex);
            }
            return result;
        }
        public IEnumerable<RoomFacilitySnapshot> GetAllRoomFacilityDetails()
        {
            IEnumerable<RoomFacilitySnapshot> result = null;
            try
            {
                result = this.GetAll();
            }
            catch (Exception ex)
            {
                _tracer.LogError("Reading Roomfacility Details", ex);
            }
            return result;
        }
        public long AddRoomFacility(RoomFacilitySnapshot _Roomfacility, bool Commit = true)
        {
            long returnValue = 0;
            try
            {
                this.Add(_Roomfacility);
                if (Commit)
                    this.UnitOfWork.CommitAndRefreshChanges();

                returnValue = _Roomfacility.Id;
            }
            catch (Exception ex)
            {
                returnValue = 0;
                _tracer.LogError("Creating Roomfacility", ex);
            }
            return returnValue;
        }
        public bool UpdateRoomFacility(RoomFacilitySnapshot Roomfacility, bool Commit = true)
        {
            Expression<Func<RoomFacilitySnapshot, bool>> expr1 = null;
            bool returnValue = false;
            try
            {
                expr1 = s => s.Id == Roomfacility.Id;
                RoomFacilitySnapshot origValue = this.GetFiltered(expr1).FirstOrDefault();
                origValue.FacilityId = Roomfacility.FacilityId;
                origValue.RoomId = Roomfacility.RoomId;
                origValue.LastUpdatedBy = Roomfacility.LastUpdatedBy;
                origValue.LastUpdatedOn = Roomfacility.LastUpdatedOn;
                this.Modify(origValue);
                if (Commit)
                    this.UnitOfWork.CommitAndRefreshChanges();
                returnValue = true;
            }
            catch (Exception ex)
            {
                returnValue = false;
                _tracer.LogError("Updating Roomfacility", ex);
            }
            return returnValue;
        }
        public bool DeleteRoomFacility(RoomFacilitySnapshot Roomfacility, bool Commit = true)
        {
            Expression<Func<RoomFacilitySnapshot, bool>> expr1 = null;
            bool returnValue = false;
            try
            {
                expr1 = s => s.Id == Roomfacility.Id;
                RoomFacilitySnapshot origValue = this.GetFiltered(expr1).FirstOrDefault();
                origValue.IsActive = false;
                origValue.LastUpdatedBy = Roomfacility.LastUpdatedBy;
                origValue.LastUpdatedOn = Roomfacility.LastUpdatedOn;
                this.Modify(origValue);
                if (Commit)
                    this.UnitOfWork.CommitAndRefreshChanges();
                returnValue = true;
            }
            catch (Exception ex)
            {
                returnValue = false;
                _tracer.LogError("Deleting Roomfacility", ex);
            }
            return returnValue;
        }
    }
}
