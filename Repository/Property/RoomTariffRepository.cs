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
    public class RoomTariffRepository : Repository<RoomTariffSnapshot>, IRoomTariffRepository
    {
        #region Properties

        private ILogger _tracer;

        #endregion Properties

        #region Constructor
        public RoomTariffRepository(UnitOfWork unitOfWork, ILogger Tracer)
            : base(unitOfWork)
        {
            if (Tracer == null)
                throw new ArgumentNullException("Tracer");
            _tracer = Tracer;
        }

        #endregion

        public IEnumerable<RoomTariffSnapshot> GetFilteredRoomTariffDetails(Expression<Func<RoomTariffSnapshot, bool>> expr1)
        {
            IEnumerable<RoomTariffSnapshot> result = null;
            try
            {
                result = this.GetFiltered(expr1);
            }
            catch (Exception ex)
            {
                _tracer.LogError("Reading Filtered RoomTariff Details", ex);
            }
            return result;
        }
        public IEnumerable<RoomTariffSnapshot> GetAllRoomTariffDetails()
        {
            IEnumerable<RoomTariffSnapshot> result = null;
            try
            {
                result = this.GetAll();
            }
            catch (Exception ex)
            {
                _tracer.LogError("Reading RoomTariff Details", ex);
            }
            return result;
        }
        public long AddRoomTariff(RoomTariffSnapshot _RoomTariff, bool Commit = true)
        {
            long returnValue = 0;
            try
            {
                this.Add(_RoomTariff);
                if (Commit)
                    this.UnitOfWork.CommitAndRefreshChanges();

                returnValue = _RoomTariff.Id;
            }
            catch (Exception ex)
            {
                returnValue = 0;
                _tracer.LogError("Creating RoomTariff", ex);
            }
            return returnValue;
        }
        //public bool UpdateRoomTariff(RoomTariffSnapshot RoomTariff, bool Commit = true)
        //{
        //    Expression<Func<RoomTariffSnapshot, bool>> expr1 = null;
        //    bool returnValue = false;
        //    try
        //    {
        //        expr1 = s => s.Id == RoomTariff.Id;
        //        RoomTariffSnapshot origValue = this.GetFiltered(expr1).FirstOrDefault();
        //        origValue.FacilityId = RoomTariff.FacilityId;
        //        origValue.PropertyId = RoomTariff.PropertyId;
        //        origValue.LastUpdatedBy = RoomTariff.LastUpdatedBy;
        //        origValue.LastUpdatedOn = RoomTariff.LastUpdatedOn;
        //        this.Modify(origValue);
        //        if (Commit)
        //            this.UnitOfWork.CommitAndRefreshChanges();
        //        returnValue = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        returnValue = false;
        //        _tracer.LogError("Updating RoomTariff", ex);
        //    }
        //    return returnValue;
        //}
        //public bool DeleteRoomTariff(RoomTariffSnapshot RoomTariff, bool Commit = true)
        //{
        //    Expression<Func<RoomTariffSnapshot, bool>> expr1 = null;
        //    bool returnValue = false;
        //    try
        //    {
        //        expr1 = s => s.Id == RoomTariff.Id;
        //        RoomTariffSnapshot origValue = this.GetFiltered(expr1).FirstOrDefault();
        //        origValue.IsActive = false;
        //        origValue.LastUpdatedBy = RoomTariff.LastUpdatedBy;
        //        origValue.LastUpdatedOn = RoomTariff.LastUpdatedOn;
        //        this.Modify(origValue);
        //        if (Commit)
        //            this.UnitOfWork.CommitAndRefreshChanges();
        //        returnValue = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        returnValue = false;
        //        _tracer.LogError("Deleting RoomTariff", ex);
        //    }
        //    return returnValue;
        //}
    }
}
