using Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    [Table("Users")]
    public class UserSnapshot : Entity, IValidatableObject
    {
        #region Constructor
        public UserSnapshot()
        {

        }
        #endregion Constructor

        #region Property   
        [Key]     
        [Column("UserId")]
        public long UserId { get; set; }
        [Column("UserName")]
        public string UserName { get; set; }
        [Column("Name")]
        public string Name { get; set; }
        [Column("Password")]
        public string Password { get; set; }
        [Column("Email")]
        public string Email { get; set; }
        [Column("PhoneNumber")]
        public string PhoneNumber { get; set; }
        [Column("Age")]
        public int Age { get; set; }
        [Column("Gender")]
        public string Gender { get; set; }
        [Column("Nationality")]
        public string Nationality { get; set; }        
        [Column("IsActive")]
        public bool IsActive { get; set; }
        [Column("IsOwner")]
        public bool IsOwner { get; set; }
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
