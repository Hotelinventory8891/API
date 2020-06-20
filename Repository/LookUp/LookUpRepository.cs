using Common.Logging;
using Core.Lookup;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository.LookUp
{
    public class LookupRepository : Repository<LookupSnapshot>, ILookupRepository
    {
        #region Properties

        private ILogger _tracer;

        #endregion Properties


        #region Constructor
        public LookupRepository(UnitOfWork unitOfWork, ILogger Tracer)
            : base(unitOfWork)
        {
            if (Tracer == null)
                throw new ArgumentNullException("Tracer");
            _tracer = Tracer;
        }

        #endregion

        public IEnumerable<LookupSnapshot> GetFilteredLookupDetails(Expression<Func<LookupSnapshot, bool>> expr1)
        {
            IEnumerable<LookupSnapshot> result = null;
            try
            {
                result = this.GetFiltered(expr1);
            }
            catch (Exception ex)
            {
                _tracer.LogError("Reading Filtered LookupDetails", ex);
            }
            return result;
        }

        public IEnumerable<LookupSnapshot> GetAllLookupDetails()
        {
            IEnumerable<LookupSnapshot> result = null;
            try
            {
                result = this.GetAll();
            }
            catch (Exception ex)
            {
                _tracer.LogError("Reading LookupDetails", ex);
            }
            return result;
        }

    }
    public class LookupTypeRepository : Repository<LookupTypeSnapshot>, ILookupTypeRepository
    {
        #region Properties

        private ILogger _tracer;

        #endregion Properties


        #region Constructor
        public LookupTypeRepository(UnitOfWork unitOfWork, ILogger Tracer)
            : base(unitOfWork)
        {
            if (Tracer == null)
                throw new ArgumentNullException("Tracer");
            _tracer = Tracer;
        }

        #endregion

        public IEnumerable<LookupTypeSnapshot> GetFilteredLookupTypeDetails(Expression<Func<LookupTypeSnapshot, bool>> expr1)
        {
            IEnumerable<LookupTypeSnapshot> result = null;
            try
            {
                result = this.GetFiltered(expr1);
            }
            catch (Exception ex)
            {
                _tracer.LogError("Reading Filtered LookupTypeDetails", ex);
            }
            return result;
        }

        public IEnumerable<LookupTypeSnapshot> GetAllLookupTypeDetails()
        {
            IEnumerable<LookupTypeSnapshot> result = null;
            try
            {
                result = this.GetAll();
            }
            catch (Exception ex)
            {
                _tracer.LogError("Reading LookupTypeDetails", ex);
            }
            return result;
        }

    }
    public class LookupViewRepository : Repository<LookUpViewSnapshot>, ILookupViewRepository
    {
        #region Properties

        private ILogger _tracer;

        #endregion Properties


        #region Constructor
        public LookupViewRepository(UnitOfWork unitOfWork, ILogger Tracer)
            : base(unitOfWork)
        {
            if (Tracer == null)
                throw new ArgumentNullException("Tracer");
            _tracer = Tracer;
        }

        #endregion

        public IEnumerable<LookUpViewSnapshot> GetFilteredLookupViewDetails(Expression<Func<LookUpViewSnapshot, bool>> expr1)
        {
            IEnumerable<LookUpViewSnapshot> result = null;
            try
            {
                result = this.GetFiltered(expr1);
            }
            catch (Exception ex)
            {
                _tracer.LogError("Reading Filtered LookupViewDetails", ex);
            }
            return result;
        }

        public IEnumerable<LookUpViewSnapshot> GetAllLookupViewDetails()
        {
            IEnumerable<LookUpViewSnapshot> result = null;
            try
            {
                result = this.GetAll();
            }
            catch (Exception ex)
            {
                _tracer.LogError("Reading LookupViewDetails", ex);
            }
            return result;
        }

    }
}
