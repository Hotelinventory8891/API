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
    [Table("Room")]
    public class RoomSnapshot : Entity, IValidatableObject
    {
        #region Constructor
        public RoomSnapshot()
        {

        }
        #endregion Constructor

        #region Property   
        [Key]
        [Column("RoomId")]
        public long RoomId { get; set; }
        [Column("PropertyId")]
        public long PropertyId { get; set; }        
        [Column("RoomType")]
        public int RoomType { get; set; }
        [Column("Max_Adult")]
        public int Max_Adult { get; set; }
        [Column("Max_Children")]
        public int Max_Children { get; set; }
        [Column("Max_Occupant")]
        public int Max_Occupant { get; set; }
        [Column("Floor")]
        public int Floor { get; set; }
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
