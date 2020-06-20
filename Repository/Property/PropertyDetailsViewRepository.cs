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
    public class PropertyDetailsViewRepository : Repository<PropertyDetailsViewSnapshot>, IPropertyDetailsViewRepository
    {
        #region Properties

        private ILogger _tracer;

        #endregion Properties

        #region Constructor
        public PropertyDetailsViewRepository(UnitOfWork unitOfWork, ILogger Tracer)
            : base(unitOfWork)
        {
            if (Tracer == null)
                throw new ArgumentNullException("Tracer");
            _tracer = Tracer;
        }

        #endregion

        public IEnumerable<PropertyDetailsViewSnapshot> GetFilteredPropertyDetailsViewDetails(Expression<Func<PropertyDetailsViewSnapshot, bool>> expr1)
        {
            IEnumerable<PropertyDetailsViewSnapshot> result = null;
            try
            {
                result = this.GetFiltered(expr1);
            }
            catch (Exception ex)
            {
                _tracer.LogError("Reading Filtered PropertyDetailsView Details", ex);
            }
            return result;
        }
        public IEnumerable<PropertyDetailsViewSnapshot> GetAllPropertyDetailsViewDetails()
        {
            IEnumerable<PropertyDetailsViewSnapshot> result = null;
            try
            {
                result = this.GetAll();
            }
            catch (Exception ex)
            {
                _tracer.LogError("Reading PropertyDetailsView Details", ex);
            }
            return result;
        }
    }
}
