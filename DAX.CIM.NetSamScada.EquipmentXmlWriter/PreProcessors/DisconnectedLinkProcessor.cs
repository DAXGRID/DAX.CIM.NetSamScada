using DAX.CIM.PhysicalNetworkModel;
using DAX.CIM.PhysicalNetworkModel.Traversal;
using DAX.CIM.PhysicalNetworkModel.Traversal.Extensions;
using DAX.IO.CIM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DAX.CIM.NetSamScada.PreProcessors
{
    public class DisconnectedLinkProcessor : IPreProcessor
    {
        int _guidOffset = 1000;
      
        public IEnumerable<IdentifiedObject> Transform(CimContext context, IEnumerable<IdentifiedObject> input)
        {
            // New objects to be added to output
            List<IdentifiedObject> newObjects = new List<IdentifiedObject>();

            // Dictionary holding ACLS terminals that need to be reconnected to new CN
            Dictionary<Terminal, ConnectivityNode> aclsTerminalNewCn = new Dictionary<Terminal, ConnectivityNode>();

            // Before processing enumerable we use context, to figure out what needed to be changed in the stream
            foreach (var contextCimObj in context.GetAllObjects())
            {
                if (contextCimObj is PowerTransformer)
                {
                    var pt = contextCimObj as PowerTransformer;

                    if (pt.name == "31128")
                    {

                    }

                    var neighbors = pt.GetNeighborConductingEquipments(context);

                    foreach (var neighbor in neighbors)
                    {
                        // If power transformer is connected directly to an ACLS then add disconnected link switch (PSI thing) in between
                        if (neighbor is ACLineSegment && neighbor.PSRType != "InternalCable")
                        {
                            var acls = neighbor as ACLineSegment;

                            // Get the terminal of the ACLS that is connected to the power transformer
                            var aclsTerminal = acls.GetTerminal(pt, true, context);

                            // Get the terminal of the PT that is connected to the ACLS
                            var ptTerminal = pt.GetTerminal(acls, true, context);

                            // Create a new CN to be used between ACLS and DL switch
                            var newCn = new ConnectivityNode();
                            newCn.mRID = GUIDHelper.CreateDerivedGuid(Guid.Parse(acls.mRID), _guidOffset++, true).ToString();
                            newObjects.Add(newCn);

                            // Create bay to hold disconnected link switch
                            var newBay = new BayExt();
                            newBay.mRID = GUIDHelper.CreateDerivedGuid(Guid.Parse(acls.mRID), _guidOffset++, true).ToString();
                            newBay.VoltageLevel = new BayVoltageLevel() { @ref = pt.GetSubstation(true, context).GetVoltageLevel(acls.BaseVoltage, true, context).mRID };
                            newBay.name = pt.name + " DL";
                            newBay.description = "Auto generated DL Bay";
                            newObjects.Add(newBay);

                            // Create disconnected link switch
                            var newSw = new Disconnector();
                            newSw.mRID = GUIDHelper.CreateDerivedGuid(Guid.Parse(acls.mRID), _guidOffset++, true).ToString();
                            newSw.PSRType = "DisconnectingLink";
                            newSw.EquipmentContainer = new EquipmentEquipmentContainer() { @ref = newBay.mRID };
                            newSw.BaseVoltage = acls.BaseVoltage;
                            newSw.name = pt.name + " DL";
                            newSw.description = "Auto generated DL";
                            newObjects.Add(newSw);

                            // Create disconnected link terminal 1, and connected it to existing PT CN
                            var newSwT1 = new Terminal();
                            newSwT1.mRID = GUIDHelper.CreateDerivedGuid(Guid.Parse(acls.mRID), _guidOffset++, true).ToString();
                            newSwT1.phases = PhaseCode.ABC;
                            newSwT1.sequenceNumber = "1";
                            newSwT1.ConductingEquipment = new TerminalConductingEquipment() { @ref = newSw.mRID };
                            newSwT1.ConnectivityNode = new TerminalConnectivityNode() { @ref = ptTerminal.ConnectivityNode.@ref };
                            newObjects.Add(newSwT1);

                            // Create disconnected link terminal 2, and connected it to newCN
                            var newSwT2 = new Terminal();
                            newSwT2.mRID = GUIDHelper.CreateDerivedGuid(Guid.Parse(acls.mRID), _guidOffset++, true).ToString();
                            newSwT2.phases = PhaseCode.ABC;
                            newSwT2.sequenceNumber = "2";
                            newSwT2.ConductingEquipment = new TerminalConductingEquipment() { @ref = newSw.mRID };
                            newSwT2.ConnectivityNode = new TerminalConnectivityNode() { @ref = newCn.mRID };
                            newObjects.Add(newSwT2);

                            // Add instruction that the existing ACLS terminal must be changed to point to new CN
                            aclsTerminalNewCn.Add(aclsTerminal, newCn);
                        }
                    }
                }
            }

            // Loop through input and modify if needed
            foreach (var inputCimObj in input)
            {
                if (inputCimObj is Terminal && aclsTerminalNewCn.ContainsKey((Terminal)inputCimObj))
                {
                    var terminal = inputCimObj as Terminal;
                    terminal.ConnectivityNode.@ref = aclsTerminalNewCn[terminal].mRID;
                }

                yield return inputCimObj;
            }

            // Add extra object
            foreach (var newObj in newObjects)
                yield return newObj;



        }
    }

    
}
