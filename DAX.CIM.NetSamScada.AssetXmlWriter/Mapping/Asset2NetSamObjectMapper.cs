using AutoMapper;
using DAX.CIM.NetSamScada.Delta.Asset;
using DAX.CIM.PhysicalNetworkModel.Traversal;
using DAX.CIM.PhysicalNetworkModel.Traversal.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DAX.CIM.NetSamScada.AssetXmlWriter.Mapping
{
    /// <summary>
    /// NetSam mapper that use AutoMapper to map Asset objects to NetSam SCADA XML objects
    /// </summary>
    public class Asset2NetSamObjectMapper
    {
        CimContext _cimContext;

        public Asset2NetSamObjectMapper(CimContext cimContext)
        {
            _cimContext = cimContext;

            Mapper.Initialize(cfg => {
                // Map conducting equipment
                cfg.CreateMap<PhysicalNetworkModel.IdentifiedObject, Asset.IdentifiedObject>()
                .Include<PhysicalNetworkModel.Asset, Asset.Asset>()
                .Include<PhysicalNetworkModel.AssetExt, Asset.AssetExt>()
                .Include<PhysicalNetworkModel.AssetInfo, Asset.AssetInfo>()
                .Include<PhysicalNetworkModel.ProductAssetModel, Asset.ProductAssetModel>()
                .Include<PhysicalNetworkModel.Manufacturer, Asset.Manufacturer>()
                .Include<PhysicalNetworkModel.PetersenCoilInfoExt, Asset.PetersenCoilInfoExt>()
                .Include<PhysicalNetworkModel.PotentialTransformerInfoExt, Asset.PotentialTransformerInfoExt>()
                .Include<PhysicalNetworkModel.CurrentTransformerInfoExt, Asset.CurrentTransformerInfoExt>()
                .Include<PhysicalNetworkModel.PowerTransformerInfoExt, Asset.PowerTransformerInfoExt>()
                .Include<PhysicalNetworkModel.CableInfoExt, Asset.CableInfoExt>()
                .Include<PhysicalNetworkModel.OverheadWireInfoExt, Asset.OverheadWireInfoExt>()
                .Include<PhysicalNetworkModel.SwitchInfoExt, Asset.SwitchInfoExt>()
                .Include<PhysicalNetworkModel.BusbarSectionInfo, Asset.BusbarSectionInfo>();

                // CoordinateSystem
                cfg.CreateMap<PhysicalNetworkModel.CoordinateSystem, Equipment.CoordinateSystem>();

                // Asset
                cfg.CreateMap<PhysicalNetworkModel.Asset, Asset.Asset>()
                    .ForMember(x => x.OrganisationRoles, opt => opt.Ignore());

                // Asset
                cfg.CreateMap<PhysicalNetworkModel.AssetExt, Asset.AssetExt>()
                    .ForMember(x => x.OrganisationRoles, opt => opt.Ignore());

                cfg.CreateMap<PhysicalNetworkModel.LifecycleDate, Asset.LifecycleDate>();
                cfg.CreateMap<PhysicalNetworkModel.AssetAssetInfo, Asset.AssetAssetInfo>();
                cfg.CreateMap<PhysicalNetworkModel.AssetInfoAssetModel, Asset.AssetInfoAssetModel>();

                //cfg.CreateMap<PhysicalNetworkModel.PowerSystemResourceAssets, Asset.PowerSystemResourceAssets>();

                // Asset Info
                cfg.CreateMap<PhysicalNetworkModel.AssetInfo, Asset.AssetInfo>();

                // Asset Info's
                cfg.CreateMap<PhysicalNetworkModel.CableInfoExt, Asset.CableInfoExt>();
                cfg.CreateMap<PhysicalNetworkModel.OverheadWireInfoExt, Asset.OverheadWireInfoExt>();
                cfg.CreateMap<PhysicalNetworkModel.SwitchInfoExt, Asset.SwitchInfoExt>();
                cfg.CreateMap<PhysicalNetworkModel.BusbarSectionInfo, Asset.BusbarSectionInfo>();
                cfg.CreateMap < PhysicalNetworkModel.PotentialTransformerInfoExt, Asset.PotentialTransformerInfoExt> ();
                cfg.CreateMap < PhysicalNetworkModel.CurrentTransformerInfoExt, Asset.CurrentTransformerInfoExt> ();
                cfg.CreateMap < PhysicalNetworkModel.PowerTransformerInfoExt, Asset.PowerTransformerInfoExt> ();
                cfg.CreateMap < PhysicalNetworkModel.PetersenCoilInfoExt, Asset.PetersenCoilInfoExt> ();


                // ProductAssetModel
                cfg.CreateMap<PhysicalNetworkModel.ProductAssetModel, Asset.ProductAssetModel>();
                cfg.CreateMap<PhysicalNetworkModel.ProductAssetModelManufacturer, Asset.ProductAssetModelManufacturer>();


                // Manufacturer
                cfg.CreateMap<PhysicalNetworkModel.Manufacturer, Asset.Manufacturer>();



                //cfg.CreateMap<PhysicalNetworkModel.StreetAddress, Asset.StreetAddress>();
                //cfg.CreateMap<PhysicalNetworkModel.StreetDetail, Asset.StreetDetail>();


                // Various reference types
                cfg.CreateMap<PhysicalNetworkModel.EquipmentEquipmentContainer, Equipment.EquipmentEquipmentContainer>();

                // Various property types
                cfg.CreateMap<PhysicalNetworkModel.Voltage, Asset.Voltage>()
                  .ForMember(dest => dest.unitSpecified, opt => opt.UseValue<bool>(true))
                    .ForMember(dest => dest.multiplierSpecified, opt => opt.UseValue<bool>(true));

                cfg.CreateMap<PhysicalNetworkModel.ApparentPower, Asset.ApparentPower>()
                  .ForMember(dest => dest.unitSpecified, opt => opt.UseValue<bool>(true))
                    .ForMember(dest => dest.multiplierSpecified, opt => opt.UseValue<bool>(true));

                cfg.CreateMap<PhysicalNetworkModel.Reactance, Asset.Reactance>()
                  .ForMember(dest => dest.unitSpecified, opt => opt.UseValue<bool>(true))
                    .ForMember(dest => dest.multiplierSpecified, opt => opt.UseValue<bool>(true));

                cfg.CreateMap<PhysicalNetworkModel.Resistance, Asset.Resistance>()
                  .ForMember(dest => dest.unitSpecified, opt => opt.UseValue<bool>(true))
                    .ForMember(dest => dest.multiplierSpecified, opt => opt.UseValue<bool>(true));

                cfg.CreateMap<PhysicalNetworkModel.Conductance, Asset.Conductance>()
                  .ForMember(dest => dest.unitSpecified, opt => opt.UseValue<bool>(true))
                    .ForMember(dest => dest.multiplierSpecified, opt => opt.UseValue<bool>(true));

                cfg.CreateMap<PhysicalNetworkModel.CurrentFlow, Asset.CurrentFlow>()
                  .ForMember(dest => dest.unitSpecified, opt => opt.UseValue<bool>(true))
                    .ForMember(dest => dest.multiplierSpecified, opt => opt.UseValue<bool>(true));

                cfg.CreateMap<PhysicalNetworkModel.PU, Asset.PU>()
                  .ForMember(dest => dest.unitSpecified, opt => opt.UseValue<bool>(true))
                    .ForMember(dest => dest.multiplierSpecified, opt => opt.UseValue<bool>(true));

                cfg.CreateMap<PhysicalNetworkModel.Susceptance, Asset.Susceptance>()
                  .ForMember(dest => dest.unitSpecified, opt => opt.UseValue<bool>(true))
                    .ForMember(dest => dest.multiplierSpecified, opt => opt.UseValue<bool>(true));

            });

        }

        /// <summary>
        /// Returns the corrosponding XML object form PNM object
        /// </summary>
        /// <param name="pnmObj"></param>
        /// <returns></returns>
        public Asset.IdentifiedObject MapObject(AssetMappingContext mappingContext, PhysicalNetworkModel.IdentifiedObject pnmObj)
        {
            HashSet<string> IncludeList = new HashSet<string>() { "PetersenCoilInfoExt", "PotentialTransformerInfoExt", "Asset", "AssetExt", "AssetInfo", "CurrentTransformerInfoExt", "PowerTransformerInfoExt", "Owner", "Manufacturer", "ProductAssetModel", "CableInfoExt", "OverheadWireInfoExt", "SwitchInfoExt", "BusbarSectionInfo" };

            Asset.IdentifiedObject netSamObj = null;

          

            try
            {
                if (IncludeList.Contains(pnmObj.GetType().Name))
                    netSamObj = Mapper.Map<Asset.IdentifiedObject>(pnmObj);
            }
            catch (Exception ex)
            {
                throw new Exception("Error mapping " + pnmObj.GetType().Name, ex);
                //return null;
            }

            // Handle asset to psr relation, owner and maintainer
            if (pnmObj is PhysicalNetworkModel.Asset)
            {
                var asset = pnmObj as PhysicalNetworkModel.Asset;
                
                if (mappingContext.AssetToPsrRefs.ContainsKey(asset.mRID))
                {
                    var psrMrid = mappingContext.AssetToPsrRefs[asset.mRID];
                    ((Asset.Asset)netSamObj).PowerSystemResources = new Asset.AssetPowerSystemResources[] { new Asset.AssetPowerSystemResources() { @ref = psrMrid } };
                }

                List<Asset.AssetOrganisationRoles> orgRoles = new List<Asset.AssetOrganisationRoles>();

                if (asset.owner != null)
                {
                    orgRoles.Add(new Asset.AssetOrganisationRoles() { @ref = mappingContext.GetOrCreateAssetOwnerByName(asset.owner).mRID });
                }

                if (asset.maintainer != null)
                {
                    orgRoles.Add(new Asset.AssetOrganisationRoles() { @ref = mappingContext.GetOrCreateAssetMaintainerByName (asset.maintainer).mRID });
                }

                ((Asset.Asset)netSamObj).OrganisationRoles = orgRoles.ToArray();



            }


            return netSamObj;
        }

        public PropertyModification MapProperty(AssetMappingContext mapContext, string propertyName, object valueObject)
        {
            if (valueObject == null)
            {
                return new PropertyModification() { Name = propertyName };
            }

            // References
            if (valueObject is PhysicalNetworkModel.AssetAssetInfo)
                return new PropertyModification() { Name = propertyName, Ref = ((PhysicalNetworkModel.AssetAssetInfo)valueObject).@ref };

            // FIX: asset replacement... but not implemented in GIS anyway

            if (valueObject is Boolean || valueObject is String)
                return new PropertyModification() { Name = propertyName, Value = valueObject };


            if (valueObject is PhysicalNetworkModel.Voltage)
                return new PropertyModification() { Name = propertyName, Value = Mapper.Map<Asset.Voltage>(valueObject) };
            if (valueObject is PhysicalNetworkModel.ApparentPower)
                return new PropertyModification() { Name = propertyName, Value = Mapper.Map<Asset.ApparentPower>(valueObject) };
            if (valueObject is PhysicalNetworkModel.Reactance)
                return new PropertyModification() { Name = propertyName, Value = Mapper.Map<Asset.Reactance>(valueObject) };
            if (valueObject is PhysicalNetworkModel.Resistance)
                return new PropertyModification() { Name = propertyName, Value = Mapper.Map<Asset.Resistance>(valueObject) };
            if (valueObject is PhysicalNetworkModel.Conductance)
                return new PropertyModification() { Name = propertyName, Value = Mapper.Map<Asset.Conductance>(valueObject) };
            if (valueObject is PhysicalNetworkModel.CurrentFlow)
                return new PropertyModification() { Name = propertyName, Value = Mapper.Map<Asset.CurrentFlow>(valueObject) };
            if (valueObject is PhysicalNetworkModel.PU)
                return new PropertyModification() { Name = propertyName, Value = Mapper.Map<Asset.PU>(valueObject) };
            if (valueObject is PhysicalNetworkModel.Susceptance)
                return new PropertyModification() { Name = propertyName, Value = Mapper.Map<Asset.Susceptance>(valueObject) };
            if (valueObject is PhysicalNetworkModel.LifecycleDate)
                return new PropertyModification() { Name = propertyName, Value = Mapper.Map<Asset.LifecycleDate>(valueObject) };


            //throw new Exception("Error mapping " + valueObject.GetType().Name);
            return null;

        }



    }
}
