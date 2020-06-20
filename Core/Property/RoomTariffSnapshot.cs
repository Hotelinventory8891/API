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
    [Table("Room_Per_Date_Tariff")]
    public class RoomTariffSnapshot : Entity, IValidatableObject
    {
        #region Constructor
        public RoomTariffSnapshot()
        {

        }
        #endregion Constructor

        #region Property   
        [Key]
        [Column("Id")]
        public long Id { get; set; }
        [Column("RoomId")]
        public long RoomId { get; set; }
        [Column("StartDate")]
        public DateTime StartDate { get; set; }
        [Column("EndDate")]
        public DateTime EndDate { get; set; }
        [Column("Tariff")]
        public decimal Tariff { get; set; }
        [Column("Discount_Percent")]
        public decimal Discount_Percent { get; set; }
        [Column("IsDefault")]
        public bool IsDefault { get; set; }
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
