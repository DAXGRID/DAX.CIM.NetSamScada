using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAX.CIM.PhysicalNetworkModel;
using DAX.CIM.PhysicalNetworkModel.Traversal;
using DAX.CIM.PhysicalNetworkModel.Traversal.Extensions;
using DAX.IO.CIM;

namespace DAX.CIM.NetSamScada.PreProcessors
{
    /// <summary>
    /// If a switch is related directly to a substation container, 
    /// then add a fictional bay to the substation, 
    /// and put the switch into the generated bay.
    /// This because, the NetSam PSI SCADA system don't like switches that have no bays.
    /// </summary>
    public class AddMissingBayProcessor : IPreProcessor
    {
        int _guidOffset = 1000;
        
        public IEnumerable<IdentifiedObject> Transform(CimContext context, IEnumerable<IdentifiedObject> input)
        {
            foreach (var inputCimObject in input)
            {
                if (inputCimObject is Switch)
                {
                    Switch sw = inputCimObject as Switch;

                    // Only manipulate switches that are inside substations and that has no bay
                    if (sw.IsInsideSubstation() && !sw.HasBay())
                    {
                        // Create fictional bay
                        var newBay = new BayExt();
                        newBay.mRID = GUIDHelper.CreateDerivedGuid(Guid.Parse(sw.mRID), _guidOffset + 1, true).ToString();
                        newBay.name = sw.name + " bay";
                        newBay.description = "Auto generated bay";

                        // Get the substation voltage level that the bay must reference
                        var vl = sw.GetSubstation().GetVoltageLevel(sw.BaseVoltage);
                        newBay.VoltageLevel = new BayVoltageLevel() { @ref = vl.mRID };

                        // Modify switch to point to new bay
                        sw.EquipmentContainer.@ref = newBay.mRID;

                        yield return newBay;
                    }
                }

                yield return inputCimObject;
            }
        }

        private Bay CreateBay(Substation st)
        {
            return null;
        }
    }
}
