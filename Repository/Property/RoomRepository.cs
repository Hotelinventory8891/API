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
    public class RoomRepository : Repository<RoomSnapshot>, IRoomRepository
    {
        #region Properties

        private ILogger _tracer;

        #endregion Properties


        #region Constructor
        public RoomRepository(UnitOfWork unitOfWork, ILogger Tracer)
            : base(unitOfWork)
        {
            if (Tracer == null)
                throw new ArgumentNullException("Tracer");
            _tracer = Tracer;
        }

        #endregion

        public IEnumerable<RoomSnapshot> GetFilteredRoomDetails(Expression<Func<RoomSnapshot, bool>> expr1)
        {
            IEnumerable<RoomSnapshot> result = null;
            try
            {
                result = this.GetFiltered(expr1);
            }
            catch (Exception ex)
            {
                _tracer.LogError("Reading Filtered Room Details", ex);
            }
            return result;
        }
        public IEnumerable<RoomSnapshot> GetAllRoomDetails()
        {
            IEnumerable<RoomSnapshot> result = null;
            try
            {
                result = this.GetAll();
            }
            catch (Exception ex)
            {
                _tracer.LogError("Reading Room Details", ex);
            }
            return result;
        }
        public long AddRoom(RoomSnapshot _Room, bool Commit = true)
        {
            long returnValue = 0;
            try
            {
                this.Add(_Room);
                if (Commit)
                    this.UnitOfWork.CommitAndRefreshChanges();

                returnValue = _Room.RoomId;
            }
            catch (Exception ex)
            {
                returnValue = 0;
                _tracer.LogError("Creating Room", ex);
            }
            return returnValue;
        }
        public bool UpdateRoom(RoomSnapshot Room, bool Commit = true)
        {
            Expression<Func<RoomSnapshot, bool>> expr1 = null;
            bool returnValue = false;
            try
            {
                expr1 = s => s.RoomId == Room.RoomId;
                RoomSnapshot origValue = this.GetFiltered(expr1).FirstOrDefault();
                origValue.PropertyId = Room.PropertyId;
                origValue.RoomType = Room.RoomType;
                origValue.Max_Adult = Room.Max_Adult;
                origValue.Max_Children = Room.Max_Children;
                origValue.Max_Occupant = Room.Max_Occupant;
                origValue.Floor = Room.Floor;
                origValue.LastUpdatedBy = Room.LastUpdatedBy;
                origValue.LastUpdatedOn = Room.LastUpdatedOn;
                this.Modify(origValue);
                if (Commit)
                    this.UnitOfWork.CommitAndRefreshChanges();
                returnValue = true;
            }
            catch (Exception ex)
            {
                returnValue = false;
                _tracer.LogError("Updating Room", ex);
            }
            return returnValue;
        }
        public bool DeleteRoom(RoomSnapshot Room, bool Commit = true)
        {
            Expression<Func<RoomSnapshot, bool>> expr1 = null;
            bool returnValue = false;
            try
            {
                expr1 = s => s.RoomId == Room.RoomId;
                RoomSnapshot origValue = this.GetFiltered(expr1).FirstOrDefault();
                origValue.IsActive = false;
                origValue.LastUpdatedBy = Room.LastUpdatedBy;
                origValue.LastUpdatedOn = Room.LastUpdatedOn;
                this.Modify(origValue);
                if (Commit)
                    this.UnitOfWork.CommitAndRefreshChanges();
                returnValue = true;
            }
            catch (Exception ex)
            {
                returnValue = false;
                _tracer.LogError("Deleting Room", ex);
            }
            return returnValue;
        }
    }
}
