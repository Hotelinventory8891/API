using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Text;

namespace Common
{
    /// <summary>
    /// Base class for entities
    /// </summary>
    public abstract class Entity //: IEntityWithRelationships
    {
        #region Members

        int? _requestedHashCode;

        #endregion

        #region Properties

        #endregion

        #region Public Methods


        #endregion

        #region Overrides Methods


        public static bool operator ==(Entity left, Entity right)
        {
            if (Object.Equals(left, null))
                return (Object.Equals(right, null)) ? true : false;
            else
                return left.Equals(right);
        }

        public static bool operator !=(Entity left, Entity right)
        {
            return !(left == right);
        }

        #endregion

        //private RelationshipManager _relationships = null;
        //public RelationshipManager RelationshipManager
        //{
        //    get
        //    {
        //        if (null == _relationships)
        //            _relationships = RelationshipManager.Create(this);
        //        return _relationships;
        //    }
        //}
    }
}
