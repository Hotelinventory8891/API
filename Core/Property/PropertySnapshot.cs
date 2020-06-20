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
    [Table("Property")]
    public class PropertySnapshot : Entity, IValidatableObject
    {
        #region Constructor
        public PropertySnapshot()
        {

        }
        #endregion Constructor

        #region Property   
        [Key]
        [Column("PropertyId")]
        public long PropertyId { get; set; }
        [Column("PropertyName")]
        public string PropertyName { get; set; }
        [Column("PropertyType")]
        public int PropertyType { get; set; }
        [Column("AddressLine1")]
        public string AddressLine1 { get; set; }
        [Column("AddressLine2")]
        public string AddressLine2 { get; set; }
        [Column("AddressLine3")]
        public string AddressLine3 { get; set; }
        [Column("StateId")]
        public int StateId { get; set; }
        [Column("CountryId")]
        public int CountryId { get; set; }
        [Column("ZipCode")]
        public string ZipCode { get; set; }
        [Column("Area")]
        public string Area { get; set; }
        [Column("No_Of_Rooms")]
        public int No_Of_Rooms { get; set; }
        [Column("Check_In_Time")]
        public string Check_In_Time { get; set; }
        [Column("Check_Out_Time")]
        public string Check_Out_Time { get; set; }
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
