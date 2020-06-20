using Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Property
{
    [Table("PropertyUser")]
    public class PropertyUserSnapshot : Entity, IValidatableObject
    {
        #region Constructor
        public PropertyUserSnapshot()
        {

        }
        #endregion Constructor

        #region Property   
        [Key]
        [Column("Id")]
        public long Id { get; set; }
        [Column("PropertyId")]
        public long PropertyId { get; set; }
        [Column("OwnerId")]
        public long OwnerId { get; set; }
        [Column("IsActive")]
        public bool IsActive { get; set; }
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
