using System;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Conventions;
using Microsoft.AspNet.Identity.EntityFramework;
using DAL.Contract;
using Core;
using Core.Lookup;

namespace DAL
{
    public class UnitOfWork : IdentityDbContext<User>, IQueryableUnitOfWork, IDisposable
    {
        #region Constructor

        public UnitOfWork()
            : base("name=DBConn")
        {
            this.Configuration.ProxyCreationEnabled = true;
            this.Configuration.LazyLoadingEnabled = false;

            this.Configuration.AutoDetectChangesEnabled = false;
            this.Database.Initialize(false);
            
            //Increased timeout 
            ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = 6000;
        }

        #endregion Constructor

        #region IDbSet Members
        IDbSet<UserSnapshot> _userSnapshot { get; set; }
        IDbSet<LookupSnapshot> _lookupSnapshot { get; set; }
        IDbSet<LookupTypeSnapshot> _lookuptypeSnapshot { get; set; }

        #endregion

        #region IQueryableUnitOfWork Members

        public DbSet<T> CreateSet<T>()
            where T : class
        {
            return base.Set<T>();
        }

        public void Detach<T>(T item)
                where T : class
        {
            //Dettach old item
            base.Entry<T>(item).State = System.Data.Entity.EntityState.Detached;
        }

        public void Attach<T>(T item)
            where T : class
        {
            //attach and set as unchanged
            base.Entry<T>(item).State = System.Data.Entity.EntityState.Unchanged;
        }

        public void SetModified<T>(T item)
            where T : class
        {
            //this operation also attach item in object state manager
            base.Entry<T>(item).State = System.Data.Entity.EntityState.Modified;
        }
        public void ApplyCurrentValues<T>(T original, T current)
            where T : class
        {
            //if it is not attached, attach original and set current values
            base.Entry<T>(original).CurrentValues.SetValues(current);
        }



        public void Commit()
        {
            base.SaveChanges();
        }

        public void CommitAndRefreshChanges()
        {
            bool saveFailed = false;

            do
            {
                try
                {
                    base.SaveChanges();

                    saveFailed = false;

                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;

                    ex.Entries.ToList()
                              .ForEach(entry =>
                              {
                                  entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                              });

                }
            } while (saveFailed);

        }

        public void RollbackChanges()
        {
            // set all entities in change tracker 
            // as 'unchanged state'
            base.ChangeTracker.Entries()
                              .ToList()
                              .ForEach(entry => entry.State = System.Data.Entity.EntityState.Unchanged);
        }

        public IEnumerable<T> ExecuteQuery<T>(string sqlQuery, params object[] parameters)
        {
            //((IObjectContextAdapter)this).ObjectContext.CommandTimeout = 1800;
            ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = 6000;
            return base.Database.SqlQuery<T>(sqlQuery, parameters);
        }

        public int ExecuteCommand(string sqlCommand, params object[] parameters)
        {
            //((IObjectContextAdapter)this).ObjectContext.CommandTimeout = 1800;
            ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = 6000;
            return base.Database.ExecuteSqlCommand(sqlCommand, parameters);
        }

        #endregion

        #region DbContext Overrides

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ///*Put your Schema name here for Oracle connections*/
            //modelBuilder.HasDefaultSchema("IFS_DATA_OWNER");

            /* To allow more than 2 decimal places (EntityFramework's default behaviour is to allow 2 decimal places) */
            #region WorkUnit
            //modelBuilder.Entity<WorkUnitTableSnapshot>().Property(x => x.LOE).HasPrecision(24, 14);
            #endregion
        }
        #endregion

        #region ExecuteMethods

        public IEnumerable<TResult> ExecuteStoredProcedure<TResult>(IStoredProcedure<TResult> procedure)
        {
            return base.Database.ExecuteStoredProcedure<TResult>(procedure);
        }

        public DataSet ExecuteStoredProcedure(IStoredProcedure procedure)
        {
            return base.Database.ExecuteStoredProcedure(procedure);
        }

        public DataSet ExecuteStoredProcedure(string storedProcedureName, IEnumerable<SqlParameter> parameters)
        {
            var connectionString = base.Database.Connection.ConnectionString;
            var ds = new DataSet();

            using (var conn = new SqlConnection(connectionString))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = storedProcedureName;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                            cmd.Parameters.Add(parameter);
                    }
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(ds);
                    }
                }
            }
            return ds;
        }
        //ExecuteNonQuery with Parameter
        public int ExecuteNonQuery(string storedProcedureName, IEnumerable<SqlParameter> parameters)
        {
            var retVal = 0;
            var connectionString = base.Database.Connection.ConnectionString;
            using (var conn = new SqlConnection(connectionString))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = storedProcedureName;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                            cmd.Parameters.Add(parameter);
                    }
                    conn.Open();
                    retVal = cmd.ExecuteNonQuery();
                }
            }
            return retVal;
        }

        #endregion ExecuteMethods

        public static UnitOfWork Create()
        {
            return new UnitOfWork();
        }
    }
}
