using DAX.CIM.NetSamScada.Equipment;
using DAX.CIM.NetSamScada.EquipmentXmlWriter.Mapping;
using DAX.CIM.PhysicalNetworkModel;
using DAX.CIM.PhysicalNetworkModel.Traversal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAX.CIM.NetSamScada.EquipmentXmlWriter
{
    public class NetSamEquipmentXMLConverter
    {
        private IEnumerable<PhysicalNetworkModel.IdentifiedObject> _inputCimObjects;
        private List<IPreProcessor> _preProcessors = new List<IPreProcessor>();
        private CimContext _context;

        public NetSamEquipmentXMLConverter(IEnumerable<PhysicalNetworkModel.IdentifiedObject> cimObjects, List<IPreProcessor> preProcessors = null)
        {
            _inputCimObjects = cimObjects;
            _context = CimContext.Create(_inputCimObjects);

            if (preProcessors != null)
                _preProcessors = preProcessors;
        }

        public IEnumerable<PhysicalNetworkModel.IdentifiedObject> GetCimObjects()
        {
            var input = _inputCimObjects;
            var output = _inputCimObjects;

            foreach (var preProcessor in _preProcessors)
            {
                output = preProcessor.Transform(_context, input);
                input = output;
            }

            foreach (var obj in output)
                yield return obj;
        }

        public ProfileEnvelop GetXMLData(List<PhysicalNetworkModel.IdentifiedObject> cimObjects)
        {
            ProfileEnvelop profile = new ProfileEnvelop();

            var afterProcessing = cimObjects;

            List<Equipment.IdentifiedObject> mappedObjects = new List<Equipment.IdentifiedObject>();

            var mapper = new PNM2NetSamObjectMapper(_context);
            var mapContext = new MappingContext();

            foreach (var cimObj in afterProcessing)
            {
                var netSamObj = mapper.MapObject(mapContext, cimObj);
                mappedObjects.Add(netSamObj);
            }

            profile.CoordinateSystem = mappedObjects.OfType<Equipment.CoordinateSystem>().ToArray();
            profile.Asset = mappedObjects.OfType<Equipment.Asset>().ToArray();
            profile.Location = mappedObjects.OfType<Equipment.Location>().ToArray();
            profile.ConnectivityNode = mappedObjects.OfType<Equipment.ConnectivityNode>().ToArray();
            profile.Terminal = mappedObjects.OfType<Equipment.Terminal>().ToArray();
            profile.Substation = mappedObjects.OfType<Equipment.Substation>().ToArray();
            profile.VoltageLevel = mappedObjects.OfType<Equipment.VoltageLevel>().ToArray();
            profile.BayExt = mappedObjects.OfType<Equipment.BayExt>().ToArray();
            profile.Disconnector = mappedObjects.OfType<Equipment.Disconnector>().ToArray();
            profile.Fuse = mappedObjects.OfType<Equipment.Fuse>().ToArray();
            profile.LoadBreakSwitch = mappedObjects.OfType<Equipment.LoadBreakSwitch>().ToArray();
            profile.Breaker = mappedObjects.OfType<Equipment.Breaker>().ToArray();
            profile.GroundDisconnector = mappedObjects.OfType<Equipment.GroundDisconnector>().ToArray();
            profile.BusbarSection = mappedObjects.OfType<Equipment.BusbarSection>().ToArray();
            profile.CurrentTransformerExt = mappedObjects.OfType<Equipment.CurrentTransformerExt>().ToArray();
            profile.FaultIndicatorExt = mappedObjects.OfType < Equipment.FaultIndicatorExt> ().ToArray();
            profile.PowerTransformer = mappedObjects.OfType < Equipment.PowerTransformer> ().ToArray();
            profile.PowerTransformerEndExt = mappedObjects.OfType < Equipment.PowerTransformerEndExt> ().ToArray();
            profile.RatioTapChanger = mappedObjects.OfType < Equipment.RatioTapChanger> ().ToArray();
            profile.AsynchronousMachine = mappedObjects.OfType < Equipment.AsynchronousMachine> ().ToArray();
            profile.SynchronousMachine = mappedObjects.OfType < Equipment.SynchronousMachine> ().ToArray();
            profile.EnergyConsumer = mappedObjects.OfType < Equipment.EnergyConsumer> ().ToArray();
            profile.UsagePoint = mappedObjects.OfType < Equipment.UsagePoint> ().ToArray();
            profile.ACLineSegmentExt = mappedObjects.OfType<Equipment.ACLineSegmentExt>().ToArray();

            // From context
            profile.PSRType = mapContext.PSRTypes.ToArray();
            profile.BaseVoltage = mapContext.BaseVoltages.ToArray();
            profile.AssetOwner = mapContext.AssetOwners.ToArray();
            profile.Maintainer = mapContext.AssetMaintainers.ToArray();
            profile.PositionPoint = mapContext.PositionPoints.ToArray();


            return profile;
        }
    }
}
