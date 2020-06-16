using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Application.Web.Models
{
    public class EnumerationModel
    {
        public string PositionName { get; set; }
        public string PositionNameID { get; set; }

        public int PositionRank { get; set; }
    }

    public class comboBindingModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class ActivityTypeModel
    {
        public long ID { get; set; }
        public string ActivityType { get; set; }
        public double LaborRate { get; set; }
        public string LaborGrade { get; set; }
    }
}