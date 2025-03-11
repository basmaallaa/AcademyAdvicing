using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.Enums
{
    [Flags]
    public enum AcadmicYear
    {
        
        None = 0,
            FirstYear = 1,
            SecondYear = 2,
            ThirdYear = 4,
            FourthYear = 8
        

    }
}
