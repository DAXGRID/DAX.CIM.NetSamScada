using DAX.CIM.NetSamScada.Equipment;
using DAX.CIM.PhysicalNetworkModel;
using DAX.CIM.PhysicalNetworkModel.Traversal;
using DAX.CIM.NetSamScada.Asset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAX.CIM.NetSamScada.AssetXmlWriter.Mapping;

namespace DAX.CIM.NetSamScada.AssetXmlWriter
{
    public class NetSamAssetXMLConverter
    {
        private IEnumerable<PhysicalNetworkModel.IdentifiedObject> _inputCimObjects;
        private CimContext _context;


        public NetSamAssetXMLConverter(IEnumerable<PhysicalNetworkModel.IdentifiedObject> cimObjects)
        {
            _inputCimObjects = cimObjects;
            _context = CimContext.Create(_inputCimObjects);
        }

        public IEnumerable<PhysicalNetworkModel.IdentifiedObject> GetCimObjects()
        {
            return  _inputCimObjects;
        }

        public AssetProfile GetXMLData(List<PhysicalNetworkModel.IdentifiedObject> cimObjects)
        {
            AssetProfile profile = new AssetProfile();

            var afterProcessing = cimObjects;

            List<Asset.IdentifiedObject> mappedObjects = new List<Asset.IdentifiedObject>();

            var mapper = new Asset2NetSamObjectMapper(_context);
            var mapContext = new AssetMappingContext(cimObjects);

            // createa asset to psr lookup table
            foreach (var cimObj in afterProcessing)
            {
                if (cimObj is PhysicalNetworkModel.PowerSystemResource)
                {
                    var psr = cimObj as PhysicalNetworkModel.PowerSystemResource;

                    if (psr.Assets != null && psr.Assets.@ref != null)
                        mapContext.AssetToPsrRefs.Add(psr.Assets.@ref, psr.mRID);
                }
            }

            foreach (var cimObj in afterProcessing)
            {
                var netSamObj = mapper.MapObject(mapContext, cimObj);

                if (netSamObj != null)
                    mappedObjects.Add(netSamObj);
            }

            profile.AssetExt = mappedObjects.OfType<Asset.AssetExt>().ToArray();
            profile.Manufacturer = mappedObjects.OfType<Asset.Manufacturer>().ToArray();
            profile.ProductAssetModel = mappedObjects.OfType<Asset.ProductAssetModel>().ToArray();

            profile.PetersenCoilInfoExt = mappedObjects.OfType<Asset.PetersenCoilInfoExt>().ToArray();
            profile.PowerTransformerInfoExt = mappedObjects.OfType<Asset.PowerTransformerInfoExt>().ToArray();
            profile.CableInfoExt = mappedObjects.OfType<Asset.CableInfoExt>().ToArray();
            profile.OverheadWireInfoExt = mappedObjects.OfType<Asset.OverheadWireInfoExt>().ToArray();
            profile.SwitchInfoExt = mappedObjects.OfType<Asset.SwitchInfoExt>().ToArray();
            profile.BusbarSectionInfo = mappedObjects.OfType<Asset.BusbarSectionInfo>().ToArray();
            profile.PotentialTransformerInfoExt = mappedObjects.OfType<Asset.PotentialTransformerInfoExt>().ToArray();
            profile.CurrentTransformerInfoExt = mappedObjects.OfType<Asset.CurrentTransformerInfoExt>().ToArray();

            // From context
            profile.AssetOwner = mapContext.AssetOwners.ToArray();
            profile.Maintainer = mapContext.AssetMaintainers.ToArray();
            

            return profile;
        }
    }
}
