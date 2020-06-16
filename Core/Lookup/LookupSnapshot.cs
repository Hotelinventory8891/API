using Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Lookup
{
    [Table("LookUp")]
    public class LookupSnapshot : Entity, IValidatableObject
    {
        #region Constructor
        public LookupSnapshot ()
        {

        }
        #endregion Constructor

        #region Property   
        [Key]
        [Column("LookUpId")]
        public long LookUpId { get; set; }
        [Column("LookUp")]
        public string LookUp { get; set; }
        [Column("LookUpTypeId")]
        public long LookUpTypeId { get; set; }
        [Column("CreatedBy")]
        public long CreatedBy { get; set; }
        [Column("CreatedOn")]
        public DateTime CreatedOn { get; set; }
        [Column("LastUpdatedBy")]
        public long LastUpdatedBy { get; set; }
        [Column("LastUpdatedOn")]
        public DateTime LastUpdatedOn { get; set; }
        #endregion Property

        #region Public Methods
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return Enumerable.Empty<ValidationResult>();
        }
        #endregion Public Methods
    }
}
