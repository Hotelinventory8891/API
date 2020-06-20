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
    public class RoomDetailsViewRepository : Repository<RoomDetailsViewSnapshot>, IRoomDetailsViewRepository
    {
        #region Properties

        private ILogger _tracer;

        #endregion Properties

        #region Constructor
        public RoomDetailsViewRepository(UnitOfWork unitOfWork, ILogger Tracer)
            : base(unitOfWork)
        {
            if (Tracer == null)
                throw new ArgumentNullException("Tracer");
            _tracer = Tracer;
        }

        #endregion

        public IEnumerable<RoomDetailsViewSnapshot> GetFilteredRoomDetailsViewDetails(Expression<Func<RoomDetailsViewSnapshot, bool>> expr1)
        {
            IEnumerable<RoomDetailsViewSnapshot> result = null;
            try
            {
                result = this.GetFiltered(expr1);
            }
            catch (Exception ex)
            {
                _tracer.LogError("Reading Filtered RoomDetailsView Details", ex);
            }
            return result;
        }
        public IEnumerable<RoomDetailsViewSnapshot> GetAllRoomDetailsViewDetails()
        {
            IEnumerable<RoomDetailsViewSnapshot> result = null;
            try
            {
                result = this.GetAll();
            }
            catch (Exception ex)
            {
                _tracer.LogError("Reading RoomDetailsView Details", ex);
            }
            return result;
        }
    }
}
