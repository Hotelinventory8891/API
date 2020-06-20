using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Lookup
{
    [Table("vw_LookUp")]
    public class LookUpViewSnapshot
    {
        #region Constructor
        public LookUpViewSnapshot()
        {

        }
        #endregion Constructor

        #region Property   
        [Key]
        [Column("LookUpId")]
        public long LookUpId { get; set; }        
        [Column("LookUpTypeId")]
        public long LookUpTypeId { get; set; }
        [Column("LookUpType")]
        public string LookUpType { get; set; }
        [Column("LookUp")]
        public string LookUp { get; set; }
        [Column("LookupDescription")]
        public string LookupDescription { get; set; }
        #endregion Property

        #region Public Methods
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return Enumerable.Empty<ValidationResult>();
        }
        #endregion Public Methods
    }
}
