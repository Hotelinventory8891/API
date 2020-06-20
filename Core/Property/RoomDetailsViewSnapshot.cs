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
    [Table("vw_RoomDetails")]
    public class RoomDetailsViewSnapshot : Entity, IValidatableObject
    {
        #region Constructor
        public RoomDetailsViewSnapshot()
        {

        }
        #endregion Constructor

        #region Property   
        [Key]
        [Column("RoomId")]
        public long RoomId { get; set; }
        [Column("PropertyId")]
        public long PropertyId { get; set; }
        [Column("RoomTypeId")]
        public int RoomTypeId { get; set; }
        [Column("RoomType")]
        public string RoomType { get; set; }
        [Column("RoomFacilities")]
        public string RoomFacilities { get; set; }
        #endregion Property

        #region Public Methods
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return Enumerable.Empty<ValidationResult>();
        }
        #endregion Public Methods
    }
}
