using DAX.CIM.PhysicalNetworkModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAX.CIM.NetSamScada.EquipmentXmlWriter.Tests
{
    public static class CimObjUniquenessChecker
    {
        public static bool IsUnique(IEnumerable<IdentifiedObject> cimObjects)
        {
            HashSet<string> mRIDs = new HashSet<string>();

            foreach (var cimObj in cimObjects)
            {
                if (mRIDs.Contains(cimObj.mRID))
                    return false;

                mRIDs.Add(cimObj.mRID);
            }

            return true;
        }
    }
}
