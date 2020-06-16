using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Core;
using DAL;
using System.Linq;
using DAL.Contract;
using System.Collections;
using System.Linq.Dynamic;
using Common.Logging;


namespace Repository
{
    /// <summary>
    /// </summary>
    public class UserRepository : Repository<UserSnapshot>, IUserRepository
    {
        #region Properties

        private ILogger _tracer;

        #endregion Properties


        #region Constructor
        public UserRepository(UnitOfWork unitOfWork, ILogger Tracer)
            : base(unitOfWork)
        {
            if (Tracer == null)
                throw new ArgumentNullException("Tracer");
            _tracer = Tracer;
        }

        #endregion

        public IEnumerable<UserSnapshot> GetFilteredUserDetails(Expression<Func<UserSnapshot, bool>> expr1)
        {
            IEnumerable<UserSnapshot> result = null;
            try
            {
                result = this.GetFiltered(expr1);
            }
            catch (Exception ex)
            {
                _tracer.LogError("Reading UserDetails", ex);
            }
            return result;
        }

        public IEnumerable<UserSnapshot> GetAllUserDetails()
        {
            IEnumerable<UserSnapshot> result = null;
            try
            {
                result = this.GetAll();
            }
            catch (Exception ex)
            {
                _tracer.LogError("Reading UserDetails", ex);
            }
            return result;
        }
        public bool AddUser(UserSnapshot _User, bool Commit = true)
        {
            bool returnValue = false;
            try
            {
                this.Add(_User);
                if (Commit)
                    this.UnitOfWork.CommitAndRefreshChanges();
                returnValue = true;
            }
            catch (Exception ex)
            {
                returnValue = false;
                _tracer.LogError("Creating User Details", ex);
            }
            return returnValue;
        }
        public bool DeleteUser(UserSnapshot _User, bool Commit = true)
        {
            bool returnValue = false;
            Expression<Func<UserSnapshot, bool>> expr1;
            try
            {
                expr1 = (w => w.UserId == _User.UserId && w.IsActive == true);
                UserSnapshot origValue = this.GetFiltered(expr1).SingleOrDefault();

                origValue.IsActive = false;
                origValue.LastUpdatedOn = _User.LastUpdatedOn;  
                origValue.LastUpdatedBy = _User.LastUpdatedBy;

                this.Modify(origValue);
                if (Commit)
                    this.UnitOfWork.CommitAndRefreshChanges();

                returnValue = true;
            }
            catch (Exception ex)
            {
                returnValue = false;
                _tracer.LogError("Updating User Details", ex);
            }
            return returnValue;
        }

    }
}
