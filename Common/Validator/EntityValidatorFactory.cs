﻿
namespace Common.Validator
{
    /// <summary>
    /// Entity Validator Factory
    /// </summary>
    public static class EntityValidatorFactory
    {
        #region Members

        static IEntityValidatorFactory _factory = null;

        #endregion

        #region Public Methods

        /// <summary>
        /// Set the  log factory to use
        /// </summary>
        /// <param name="factory">Log factory to use</param>
        public static void SetCurrent(IEntityValidatorFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Createt a new 
        /// </summary>
        /// <returns>Created ILog</returns>
        public static IEntityValidator CreateValidator()
        {
            return (_factory != null) ? _factory.Create() : null;
        }

        #endregion
    }
}
