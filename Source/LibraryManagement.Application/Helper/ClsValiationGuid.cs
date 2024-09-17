using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Helper
{
    internal class ClsValiationGuid
    {
        public bool CheckGuidStatus(Guid guid)
        {

            if (guid == Guid.Empty)
            {
                return false;
            }
            return true;

        }
    }
}
