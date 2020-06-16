using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public enum ValidateStatus
    {
        InProgress = 1,
        Success = 2,
        SchemaError = 3,
        SystemError = 4
    }

    public enum EnumMaster
    {
        Technology = 1,
        Role = 2,
        Profile = 3,
        Priority = 4,
        Module = 5,
        FileType = 6,
        ReasonCode = 7,
        DataFrom = 8,
        RFDSType = 11,
        DataFromUpdateRequest = 17,
        ReasonCodeUpdateRequest = 18,
        Type = 9
    }
    public enum Status
    {
        Started = 1,
        Processing = 2,
        Completed = 3,
        Error = 4
    }



    public enum EnumModule
    {
        Design = 1,
        RFDS = 2,
        RNDCIQ = 3
    }

    public enum ReportTypeID
    {
        CustomReport = 1
    }

    public enum SortType
    {
        Ascending = 0,
        Descending = 1
    }

    public enum ReportColumnType
    {
        String = 1,
        Int = 2,
        Decimal = 3,
        DateTime = 4
    }
}
