﻿using AutoMapper;
using DAX.CIM.NetSamScada.Delta.Equipment;
using DAX.CIM.PhysicalNetworkModel.Traversal;
using DAX.CIM.PhysicalNetworkModel.Traversal.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DAX.CIM.NetSamScada.EquipmentXmlWriter.Mapping
{
    /// <summary>
    /// NetSam mapper that use AutoMapper to map PNM objects to NetSam SCADA XML objects
    /// </summary>
    public class PNM2NetSamObjectMapper
    {
        CimContext _cimContext;

        public PNM2NetSamObjectMapper(CimContext cimContext)
        {
            _cimContext = cimContext;

            Mapper.Initialize(cfg => {
                // Map conducting equipment
                cfg.CreateMap<PhysicalNetworkModel.IdentifiedObject, Equipment.IdentifiedObject>()
                .Include<PhysicalNetworkModel.CoordinateSystem, Equipment.CoordinateSystem>()
                //.Include<PhysicalNetworkModel.Asset, Equipment.Asset>()
                .Include<PhysicalNetworkModel.LocationExt, Equipment.LocationExt>()
                .Include<PhysicalNetworkModel.ConnectivityNode, Equipment.ConnectivityNode>()
                .Include<PhysicalNetworkModel.Terminal, Equipment.Terminal>()
                .Include<PhysicalNetworkModel.Substation, Equipment.Substation>()
                .Include<PhysicalNetworkModel.VoltageLevel, Equipment.VoltageLevel>()
                .Include<PhysicalNetworkModel.BayExt, Equipment.BayExt>()
                .Include<PhysicalNetworkModel.Disconnector, Equipment.Disconnector>()
                .Include<PhysicalNetworkModel.Fuse, Equipment.Fuse>()
                .Include<PhysicalNetworkModel.LoadBreakSwitch, Equipment.LoadBreakSwitch>()
                .Include<PhysicalNetworkModel.Breaker, Equipment.Breaker>()
                .Include<PhysicalNetworkModel.GroundDisconnector, Equipment.GroundDisconnector>()
                .Include<PhysicalNetworkModel.BusbarSection, Equipment.BusbarSection>()
                .Include<PhysicalNetworkModel.ProtectionEquipmentExt,Equipment.ProtectionEquipmentExt > ()
                .Include<PhysicalNetworkModel.CurrentTransformerExt, Equipment.CurrentTransformerExt>()
                .Include<PhysicalNetworkModel.PotentialTransformer, Equipment.PotentialTransformer>()
                .Include<PhysicalNetworkModel.FaultIndicatorExt, Equipment.FaultIndicatorExt>()
                .Include<PhysicalNetworkModel.PowerTransformer, Equipment.PowerTransformer>()
                .Include<PhysicalNetworkModel.PowerTransformerEndExt, Equipment.PowerTransformerEndExt>()
                .Include<PhysicalNetworkModel.RatioTapChanger, Equipment.RatioTapChanger>()
                .Include<PhysicalNetworkModel.AsynchronousMachine, Equipment.AsynchronousMachine>()
                .Include<PhysicalNetworkModel.SynchronousMachine, Equipment.SynchronousMachine>()
                .Include<PhysicalNetworkModel.PetersenCoil, Equipment.PetersenCoil>()
                .Include<PhysicalNetworkModel.ExternalNetworkInjection, Equipment.ExternalNetworkInjection>()

                .Include<PhysicalNetworkModel.EnergyConsumer, Equipment.EnergyConsumer>()
                .Include<PhysicalNetworkModel.UsagePointExt, Equipment.UsagePointExt>()

                .Include<PhysicalNetworkModel.ACLineSegment, Equipment.ACLineSegment>()
                .Include<PhysicalNetworkModel.ACLineSegmentExt, Equipment.ACLineSegmentExt>();

                // CoordinateSystem
                cfg.CreateMap<PhysicalNetworkModel.CoordinateSystem, Equipment.CoordinateSystem>();

                // Asset
                /*
                cfg.CreateMap<PhysicalNetworkModel.Asset, Equipment.Asset>()
               .ForMember(x => x.OrganisationRoles, opt => opt.Ignore());
                cfg.CreateMap<PhysicalNetworkModel.LifecycleDate, Equipment.LifecycleDate>();
                cfg.CreateMap<PhysicalNetworkModel.StreetAddress, Equipment.StreetAddress>();
                cfg.CreateMap<PhysicalNetworkModel.StreetDetail, Equipment.StreetDetail>();
                */

                // Location
                cfg.CreateMap<PhysicalNetworkModel.LocationExt, Equipment.LocationExt>();
                cfg.CreateMap<PhysicalNetworkModel.LocationCoordinateSystem, Equipment.LocationCoordinateSystem>();

                // Connectivity Node
                cfg.CreateMap<PhysicalNetworkModel.ConnectivityNode, Equipment.ConnectivityNode>();

                // Terminal
                cfg.CreateMap<PhysicalNetworkModel.Terminal, Equipment.Terminal>();
                cfg.CreateMap<PhysicalNetworkModel.TerminalConductingEquipment, Equipment.TerminalConductingEquipment>();
                cfg.CreateMap<PhysicalNetworkModel.TerminalConnectivityNode, Equipment.TerminalConnectivityNode>();

                // Substation
                cfg.CreateMap<PhysicalNetworkModel.Substation, Equipment.Substation>()
                .ForMember(x => x.PSRType, opt => opt.Ignore());

                // Voltagelevel
                cfg.CreateMap<PhysicalNetworkModel.VoltageLevel, Equipment.VoltageLevel>()
                .ForMember(x => x.BaseVoltage, opt => opt.Ignore())
                .ForMember(x => x.PSRType, opt => opt.Ignore());

                cfg.CreateMap<PhysicalNetworkModel.VoltageLevelEquipmentContainer, Equipment.VoltageLevelEquipmentContainer>();

                // BayExt
                cfg.CreateMap<PhysicalNetworkModel.BayExt, Equipment.BayExt>()
                .ForMember(x => x.PSRType, opt => opt.Ignore());
                cfg.CreateMap<PhysicalNetworkModel.BayVoltageLevel, Equipment.BayVoltageLevel>();

                // Disconnector
                cfg.CreateMap<PhysicalNetworkModel.Disconnector, Equipment.Disconnector>()
                .ForMember(x => x.BaseVoltage, opt => opt.Ignore())
                .ForMember(x => x.PSRType, opt => opt.Ignore());

                // Fuse
                cfg.CreateMap<PhysicalNetworkModel.Fuse, Equipment.Fuse>()
                .ForMember(x => x.BaseVoltage, opt => opt.Ignore())
                .ForMember(x => x.PSRType, opt => opt.Ignore());

                // LoadBreakSwitch
                cfg.CreateMap<PhysicalNetworkModel.LoadBreakSwitch, Equipment.LoadBreakSwitch>()
                .ForMember(x => x.BaseVoltage, opt => opt.Ignore())
                .ForMember(x => x.PSRType, opt => opt.Ignore());

                // Breaker
                cfg.CreateMap<PhysicalNetworkModel.Breaker, Equipment.Breaker>()
                .ForMember(x => x.BaseVoltage, opt => opt.Ignore())
                .ForMember(x => x.PSRType, opt => opt.Ignore());

                // Ground disconnector
                cfg.CreateMap<PhysicalNetworkModel.GroundDisconnector, Equipment.GroundDisconnector>()
                .ForMember(x => x.BaseVoltage, opt => opt.Ignore())
                .ForMember(x => x.PSRType, opt => opt.Ignore());

                // Busbar
                cfg.CreateMap<PhysicalNetworkModel.BusbarSection, Equipment.BusbarSection>()
                .ForMember(x => x.BaseVoltage, opt => opt.Ignore())
                .ForMember(x => x.PSRType, opt => opt.Ignore());

                // ProtectionEquipmentExt
                cfg.CreateMap<PhysicalNetworkModel.ProtectionEquipmentExt, Equipment.ProtectionEquipmentExt>()
                .ForMember(x => x.PSRType, opt => opt.Ignore());
                cfg.CreateMap<PhysicalNetworkModel.ProtectionEquipmentExtCurrentTransformers, Equipment.ProtectionEquipmentExtCurrentTransformers>();
                cfg.CreateMap<PhysicalNetworkModel.ProtectionEquipmentExtPotentialTransformers, Equipment.ProtectionEquipmentExtPotentialTransformers>();
                cfg.CreateMap<PhysicalNetworkModel.ProtectionEquipmentProtectedSwitches, Equipment.ProtectionEquipmentProtectedSwitches>();


                // CurrentTransformerExt
                cfg.CreateMap<PhysicalNetworkModel.CurrentTransformerExt, Equipment.CurrentTransformerExt>()
                .ForMember(x => x.PSRType, opt => opt.Ignore());

                // PotentialTransformer
                cfg.CreateMap<PhysicalNetworkModel.PotentialTransformer, Equipment.PotentialTransformer>()
                .ForMember(x => x.PSRType, opt => opt.Ignore());

                // FaultIndicatorExt
                cfg.CreateMap<PhysicalNetworkModel.FaultIndicatorExt, Equipment.FaultIndicatorExt>()
                .ForMember(x => x.PSRType, opt => opt.Ignore());

                // PowerTransformer
                cfg.CreateMap<PhysicalNetworkModel.PowerTransformer, Equipment.PowerTransformer>()
               .ForMember(x => x.BaseVoltage, opt => opt.Ignore())
               .ForMember(x => x.PSRType, opt => opt.Ignore());
                cfg.CreateMap<PhysicalNetworkModel.PowerTransformerEndPowerTransformer, Equipment.PowerTransformerEndPowerTransformer>();

                // PowerTransformerEndEt
                cfg.CreateMap<PhysicalNetworkModel.PowerTransformerEndExt, Equipment.PowerTransformerEndExt>()
               .ForMember(x => x.BaseVoltage, opt => opt.Ignore());
                cfg.CreateMap<PhysicalNetworkModel.TransformerEndTerminal, Equipment.TransformerEndTerminal>();

                // RatioTapChanger
                cfg.CreateMap<PhysicalNetworkModel.RatioTapChanger, Equipment.RatioTapChanger>()
               .ForMember(x => x.PSRType, opt => opt.Ignore());
                cfg.CreateMap<PhysicalNetworkModel.RatioTapChangerTransformerEnd, Equipment.RatioTapChangerTransformerEnd>();

                // AsynchronousMachine
                cfg.CreateMap<PhysicalNetworkModel.AsynchronousMachine, Equipment.AsynchronousMachine>()
               .ForMember(x => x.BaseVoltage, opt => opt.Ignore())
               .ForMember(x => x.PSRType, opt => opt.Ignore());

                // SynchronousMachine
                cfg.CreateMap<PhysicalNetworkModel.SynchronousMachine, Equipment.SynchronousMachine>()
               .ForMember(x => x.BaseVoltage, opt => opt.Ignore())
               .ForMember(x => x.PSRType, opt => opt.Ignore());

                // PetersonCoil
                cfg.CreateMap<PhysicalNetworkModel.PetersenCoil, Equipment.PetersenCoil>()
               .ForMember(x => x.BaseVoltage, opt => opt.Ignore())
               .ForMember(x => x.PSRType, opt => opt.Ignore());
                cfg.CreateMap<PhysicalNetworkModel.PetersenCoilModeKind, Equipment.PetersenCoilModeKind>();

                // ExternalNetworkInjection
                cfg.CreateMap<PhysicalNetworkModel.ExternalNetworkInjection, Equipment.ExternalNetworkInjection>()
               .ForMember(x => x.BaseVoltage, opt => opt.Ignore())
               .ForMember(x => x.PSRType, opt => opt.Ignore());

                // EnergyConsumer
                cfg.CreateMap<PhysicalNetworkModel.EnergyConsumer, Equipment.EnergyConsumer>()
               .ForMember(x => x.BaseVoltage, opt => opt.Ignore())
               .ForMember(x => x.PSRType, opt => opt.Ignore());

                // UsagePoint
                cfg.CreateMap<PhysicalNetworkModel.UsagePointExt, Equipment.UsagePointExt>();
                cfg.CreateMap<PhysicalNetworkModel.UsagePointEquipments, Equipment.UsagePointEquipments>();


                // ACLineSegment
                cfg.CreateMap<PhysicalNetworkModel.ACLineSegment, Equipment.ACLineSegment>()
                .ForMember(x => x.BaseVoltage, opt => opt.Ignore())
                .ForMember(x => x.PSRType, opt => opt.Ignore());

                // ACLineSegmentExt
                cfg.CreateMap<PhysicalNetworkModel.ACLineSegmentExt, Equipment.ACLineSegmentExt>()
               .ForMember(x => x.BaseVoltage, opt => opt.Ignore())
               .ForMember(x => x.PSRType, opt => opt.Ignore());


                // Various reference types
                cfg.CreateMap<PhysicalNetworkModel.EquipmentEquipmentContainer, Equipment.EquipmentEquipmentContainer>();
                cfg.CreateMap<PhysicalNetworkModel.PowerSystemResourceLocation, Equipment.PowerSystemResourceLocation>();
                //cfg.CreateMap<PhysicalNetworkModel.PowerSystemResourceAssets, Equipment.PowerSystemResourceAssets>();
                cfg.CreateMap<PhysicalNetworkModel.AuxiliaryEquipmentTerminal, Equipment.AuxiliaryEquipmentTerminal>();

                // Various property types
                cfg.CreateMap<PhysicalNetworkModel.Voltage, Equipment.Voltage>()
                    .ForMember(dest => dest.unitSpecified, opt => opt.UseValue<bool>(true))
                    .ForMember(dest => dest.multiplierSpecified, opt => opt.UseValue<bool>(true));

                cfg.CreateMap<PhysicalNetworkModel.ApparentPower, Equipment.ApparentPower>()
                    .ForMember(dest => dest.unitSpecified, opt => opt.UseValue<bool>(true))
                    .ForMember(dest => dest.multiplierSpecified, opt => opt.UseValue<bool>(true));

                cfg.CreateMap<PhysicalNetworkModel.ActivePower, Equipment.ActivePower>()
                    .ForMember(dest => dest.unitSpecified, opt => opt.UseValue<bool>(true))
                    .ForMember(dest => dest.multiplierSpecified, opt => opt.UseValue<bool>(true));

                cfg.CreateMap<PhysicalNetworkModel.KiloActivePower, Equipment.KiloActivePower>()
                    .ForMember(dest => dest.unitSpecified, opt => opt.UseValue<bool>(true))
                    .ForMember(dest => dest.multiplierSpecified, opt => opt.UseValue<bool>(true));

                cfg.CreateMap<PhysicalNetworkModel.ActivePowerPerFrequency, Equipment.ActivePowerPerFrequency>()
                    .ForMember(dest => dest.unitSpecified, opt => opt.UseValue<bool>(true))
                    .ForMember(dest => dest.multiplierSpecified, opt => opt.UseValue<bool>(true));

                cfg.CreateMap<PhysicalNetworkModel.Reactance, Equipment.Reactance>()
                    .ForMember(dest => dest.unitSpecified, opt => opt.UseValue<bool>(true))
                    .ForMember(dest => dest.multiplierSpecified, opt => opt.UseValue<bool>(true));

                cfg.CreateMap<PhysicalNetworkModel.Resistance, Equipment.Resistance>()
                    .ForMember(dest => dest.unitSpecified, opt => opt.UseValue<bool>(true))
                    .ForMember(dest => dest.multiplierSpecified, opt => opt.UseValue<bool>(true));

                cfg.CreateMap<PhysicalNetworkModel.Capacitance, Equipment.Capacitance>()
                    .ForMember(dest => dest.unitSpecified, opt => opt.UseValue<bool>(true))
                    .ForMember(dest => dest.multiplierSpecified, opt => opt.UseValue<bool>(true));

                cfg.CreateMap<PhysicalNetworkModel.Conductance, Equipment.Conductance>()
                    .ForMember(dest => dest.unitSpecified, opt => opt.UseValue<bool>(true))
                    .ForMember(dest => dest.multiplierSpecified, opt => opt.UseValue<bool>(true));

                cfg.CreateMap<PhysicalNetworkModel.CurrentFlow, Equipment.CurrentFlow>()
                    .ForMember(dest => dest.unitSpecified, opt => opt.UseValue<bool>(true))
                    .ForMember(dest => dest.multiplierSpecified, opt => opt.UseValue<bool>(true));

                cfg.CreateMap<PhysicalNetworkModel.Frequency, Equipment.Frequency>()
                    .ForMember(dest => dest.unitSpecified, opt => opt.UseValue<bool>(true));

                cfg.CreateMap<PhysicalNetworkModel.GroundingImpedance, Equipment.GroundingImpedance>();

                cfg.CreateMap<PhysicalNetworkModel.KiloActivePower, Equipment.KiloActivePower>()
                    .ForMember(dest => dest.unitSpecified, opt => opt.UseValue<bool>(true))
                    .ForMember(dest => dest.multiplierSpecified, opt => opt.UseValue<bool>(true));

                cfg.CreateMap<PhysicalNetworkModel.Length, Equipment.Length>()
                    .ForMember(dest => dest.unitSpecified, opt => opt.UseValue<bool>(true))
                    .ForMember(dest => dest.multiplierSpecified, opt => opt.UseValue<bool>(true));

                cfg.CreateMap<PhysicalNetworkModel.PerCent, Equipment.PerCent>()
                    .ForMember(dest => dest.unitSpecified, opt => opt.UseValue<bool>(true))
                    .ForMember(dest => dest.multiplierSpecified, opt => opt.UseValue<bool>(true));

                cfg.CreateMap<PhysicalNetworkModel.PU, Equipment.PU>()
                    .ForMember(dest => dest.unitSpecified, opt => opt.UseValue<bool>(true))
                    .ForMember(dest => dest.multiplierSpecified, opt => opt.UseValue<bool>(true));

                cfg.CreateMap<PhysicalNetworkModel.ReactivePower, Equipment.ReactivePower>()
                    .ForMember(dest => dest.unitSpecified, opt => opt.UseValue<bool>(true))
                    .ForMember(dest => dest.multiplierSpecified, opt => opt.UseValue<bool>(true));

                cfg.CreateMap<PhysicalNetworkModel.RotationSpeed, Equipment.RotationSpeed>()
                    .ForMember(dest => dest.unitSpecified, opt => opt.UseValue<bool>(true))
                    .ForMember(dest => dest.multiplierSpecified, opt => opt.UseValue<bool>(true));

                cfg.CreateMap<PhysicalNetworkModel.Seconds, Equipment.Seconds>()
                    .ForMember(dest => dest.unitSpecified, opt => opt.UseValue<bool>(true))
                    .ForMember(dest => dest.multiplierSpecified, opt => opt.UseValue<bool>(true));

                cfg.CreateMap<PhysicalNetworkModel.Status, Equipment.Status>();

                cfg.CreateMap<PhysicalNetworkModel.Susceptance, Equipment.Susceptance>()
                    .ForMember(dest => dest.unitSpecified, opt => opt.UseValue<bool>(true))
                    .ForMember(dest => dest.multiplierSpecified, opt => opt.UseValue<bool>(true));

                cfg.CreateMap<PhysicalNetworkModel.VoltagePerReactivePower, Equipment.VoltagePerReactivePower>()
                    .ForMember(dest => dest.unitSpecified, opt => opt.UseValue<bool>(true))
                    .ForMember(dest => dest.multiplierSpecified, opt => opt.UseValue<bool>(true));


            });

        }

        /// <summary>
        /// Returns the corrosponding XML object form PNM object
        /// </summary>
        /// <param name="pnmObj"></param>
        /// <returns></returns>
        public Equipment.IdentifiedObject MapObject(MappingContext mapContext, PhysicalNetworkModel.IdentifiedObject pnmObj)
        {
            HashSet<string> IgnoreList = new HashSet<string>() { "PetersenCoilInfoExt", "PotentialTransformerInfoExt", "AssetExt", "AssetInfo", "BusbarSectionInfo", "SwitchInfoExt", "CurrentTransformerInfoExt", "PowerTransformerInfoExt", "Owner", "Manufacturer", "ProductAssetModel", "CableInfoExt", "OverheadWireInfoExt" };

            Equipment.IdentifiedObject netSamObj = null;

            try
            {
                if (!IgnoreList.Contains(pnmObj.GetType().Name))
                    netSamObj = Mapper.Map<Equipment.IdentifiedObject>(pnmObj);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error mapping " + pnmObj.GetType().Name);
                //throw new Exception("Error mapping " + pnmObj.GetType().Name, ex);
                //return null;

            }

            if (netSamObj == null)
                return null;


            // Handle PSRType
            if (pnmObj is PhysicalNetworkModel.PowerSystemResource && ((PhysicalNetworkModel.PowerSystemResource)pnmObj).PSRType != null)
            {
                var psr = pnmObj as PhysicalNetworkModel.PowerSystemResource;
                var psrType = mapContext.GetOrCreatePSRTypeByName(psr.PSRType);
                ((Equipment.PowerSystemResource)netSamObj).PSRType = new Equipment.PowerSystemResourcePSRType() { @ref = psrType.mRID };
            }

            // Handle base voltage on conducting equipments
            if (pnmObj is PhysicalNetworkModel.ConductingEquipment)
            {
                var ci = pnmObj as PhysicalNetworkModel.ConductingEquipment;

                // If no voltage level, try get it from a neighboor
                // FIX: all conducting equipment except power transformer should have a base voltage.
                if (!(pnmObj is PhysicalNetworkModel.PowerTransformer || pnmObj is PhysicalNetworkModel.AsynchronousMachine || pnmObj is PhysicalNetworkModel.SynchronousMachine || pnmObj is PhysicalNetworkModel.ExternalNetworkInjection)  && ci.BaseVoltage == 0)
                {
                    
                    var neigboors = ci.GetNeighborConductingEquipments(_cimContext);

                    var neighboorWithVoltageLevel = neigboors.Find(o => o.BaseVoltage > 0);

                    // If a neighboor with voltage level found, use that one
                    if (neighboorWithVoltageLevel != null)
                        ci.BaseVoltage = neighboorWithVoltageLevel.BaseVoltage;
                    // If an orphan energy consumer, set voltage level to 400 volt
                    else if (neighboorWithVoltageLevel == null && pnmObj is PhysicalNetworkModel.EnergyConsumer)
                        ci.BaseVoltage = 400;
                    else
                        ci.BaseVoltage = 0;
                       //throw new Exception("ConductingEquipment with mRID=" + pnmObj.mRID + " BaseVoltage not set.");
                }                    

                if (ci.BaseVoltage > 0)
                {
                    var baseVoltage = mapContext.GetOrCreateBaseVoltage(ci.BaseVoltage);
                    ((Equipment.ConductingEquipment)netSamObj).BaseVoltage = new Equipment.ConductingEquipmentBaseVoltage() { @ref = baseVoltage.mRID };
                }
            }

            // Handle base voltage on voltage levels
            if (pnmObj is PhysicalNetworkModel.VoltageLevel)
            {
                if (((PhysicalNetworkModel.VoltageLevel)pnmObj).BaseVoltage == 0)
                    throw new Exception("VoltageLevel with mRID=" + pnmObj.mRID + " BaseVoltage not set.");

                var vl = pnmObj as PhysicalNetworkModel.VoltageLevel;
                var baseVoltage = mapContext.GetOrCreateBaseVoltage(vl.BaseVoltage);
                ((Equipment.VoltageLevel)netSamObj).BaseVoltage = new Equipment.VoltageLevelBaseVoltage() { @ref = baseVoltage.mRID };
            }

            // Handle base voltage on transformer ends
            if (pnmObj is PhysicalNetworkModel.TransformerEnd)
            {
                if (((PhysicalNetworkModel.TransformerEnd)pnmObj).BaseVoltage == 0)
                    throw new Exception("TransformerEnd with mRID=" + pnmObj.mRID + " BaseVoltage not set.");

                var te = pnmObj as PhysicalNetworkModel.TransformerEnd;
                var baseVoltage = mapContext.GetOrCreateBaseVoltage(te.BaseVoltage);
                ((Equipment.TransformerEnd)netSamObj).BaseVoltage = new Equipment.TransformerEndBaseVoltage() { @ref = baseVoltage.mRID };
            }

            // Handle position points
            if (pnmObj is PhysicalNetworkModel.LocationExt && ((PhysicalNetworkModel.LocationExt)pnmObj).coordinates != null)
            {
                var loc = pnmObj as PhysicalNetworkModel.LocationExt;

                Equipment.LocationExt xmlLoc = netSamObj as Equipment.LocationExt;

                List<Equipment.PositionPoint> xmlPositionPoints = new List<Equipment.PositionPoint>();

                // Add position point for each coordinate
                int seqNo = 1;
                foreach (var coord in loc.coordinates)
                {
                    var xmlPoint = mapContext.AddPositionPoint(null, seqNo, coord.X, coord.Y);
                    xmlPositionPoints.Add(xmlPoint);
                    seqNo++;
                }

                xmlLoc.positionPoints = new Equipment.PositionPoints() { PositionPoint = xmlPositionPoints.ToArray() };
            }

            return netSamObj;
        }


        public PropertyModification MapProperty(MappingContext mapContext, string propertyName, object valueObject)
        {
            if (propertyName == "coordinates")
            {

            }

            if (valueObject == null)
            {
                return new PropertyModification() { Name = propertyName };
            }


            if (valueObject is PhysicalNetworkModel.TerminalConnectivityNode)
                return new PropertyModification() { Name = propertyName, Ref = ((PhysicalNetworkModel.TerminalConnectivityNode)valueObject).@ref };

            if (valueObject is Boolean || valueObject is String)
                return new PropertyModification() { Name = propertyName, Value = valueObject };


            if (valueObject is PhysicalNetworkModel.Voltage)
                return new PropertyModification() { Name = propertyName, Value = Mapper.Map<Equipment.Voltage>(valueObject) };
            if (valueObject is PhysicalNetworkModel.ApparentPower)
                return new PropertyModification() { Name = propertyName, Value = Mapper.Map<Equipment.ApparentPower>(valueObject) };
            if (valueObject is PhysicalNetworkModel.ActivePower)
                return new PropertyModification() { Name = propertyName, Value = Mapper.Map<Equipment.ActivePower>(valueObject) };
            if (valueObject is PhysicalNetworkModel.KiloActivePower)
                return new PropertyModification() { Name = propertyName, Value = Mapper.Map<Equipment.KiloActivePower>(valueObject) };
            if (valueObject is PhysicalNetworkModel.ActivePowerPerFrequency)
                return new PropertyModification() { Name = propertyName, Value = Mapper.Map<Equipment.ActivePowerPerFrequency>(valueObject) };
            if (valueObject is PhysicalNetworkModel.Reactance)
                return new PropertyModification() { Name = propertyName, Value = Mapper.Map<Equipment.Reactance>(valueObject) };
            if (valueObject is PhysicalNetworkModel.Resistance)
                return new PropertyModification() { Name = propertyName, Value = Mapper.Map<Equipment.Resistance>(valueObject) };
            if (valueObject is PhysicalNetworkModel.Capacitance)
                return new PropertyModification() { Name = propertyName, Value = Mapper.Map<Equipment.Capacitance>(valueObject) };
            if (valueObject is PhysicalNetworkModel.Conductance)
                return new PropertyModification() { Name = propertyName, Value = Mapper.Map<Equipment.Conductance>(valueObject) };
            if (valueObject is PhysicalNetworkModel.CurrentFlow)
                return new PropertyModification() { Name = propertyName, Value = Mapper.Map<Equipment.CurrentFlow>(valueObject) };
            if (valueObject is PhysicalNetworkModel.Frequency)
                return new PropertyModification() { Name = propertyName, Value = Mapper.Map<Equipment.Frequency>(valueObject) };
            if (valueObject is PhysicalNetworkModel.GroundingImpedance)
                return new PropertyModification() { Name = propertyName, Value = Mapper.Map<Equipment.GroundingImpedance>(valueObject) };
            if (valueObject is PhysicalNetworkModel.KiloActivePower)
                return new PropertyModification() { Name = propertyName, Value = Mapper.Map<Equipment.KiloActivePower>(valueObject) };
            if (valueObject is PhysicalNetworkModel.Length)
                return new PropertyModification() { Name = propertyName, Value = Mapper.Map<Equipment.Length>(valueObject) };
            if (valueObject is PhysicalNetworkModel.PerCent)
                return new PropertyModification() { Name = propertyName, Value = Mapper.Map<Equipment.PerCent>(valueObject) };
            if (valueObject is PhysicalNetworkModel.PU)
                return new PropertyModification() { Name = propertyName, Value = Mapper.Map<Equipment.PU>(valueObject) };
            if (valueObject is PhysicalNetworkModel.ReactivePower)
                return new PropertyModification() { Name = propertyName, Value = Mapper.Map<Equipment.ReactivePower>(valueObject) };
            if (valueObject is PhysicalNetworkModel.RotationSpeed)
                return new PropertyModification() { Name = propertyName, Value = Mapper.Map<Equipment.RotationSpeed>(valueObject) };
            if (valueObject is PhysicalNetworkModel.Seconds)
                return new PropertyModification() { Name = propertyName, Value = Mapper.Map<Equipment.Seconds>(valueObject) };
            if (valueObject is PhysicalNetworkModel.Status)
                return new PropertyModification() { Name = propertyName, Value = Mapper.Map<Equipment.Status>(valueObject) };
            if (valueObject is PhysicalNetworkModel.Susceptance)
                return new PropertyModification() { Name = propertyName, Value = Mapper.Map<Equipment.Susceptance>(valueObject) };
            if (valueObject is PhysicalNetworkModel.VoltagePerReactivePower)
                return new PropertyModification() { Name = propertyName, Value = Mapper.Map<Equipment.VoltagePerReactivePower>(valueObject) };

            /*
            if (valueObject is PhysicalNetworkModel.LifecycleDate)
                return new PropertyModification() { Name = propertyName, Value = Mapper.Map<Equipment.LifecycleDate>(valueObject) };
            */
            if (valueObject is PhysicalNetworkModel.StreetAddress)
                return new PropertyModification() { Name = propertyName, Value = Mapper.Map<Equipment.StreetAddress>(valueObject) };
            if (valueObject is PhysicalNetworkModel.StreetDetail)
                return new PropertyModification() { Name = propertyName, Value = Mapper.Map<Equipment.StreetDetail>(valueObject) };


            if (valueObject is PhysicalNetworkModel.Point2D[])
            {
                var coords = valueObject as PhysicalNetworkModel.Point2D[];



                List<Equipment.PositionPoint> xmlPositionPoints = new List<Equipment.PositionPoint>();

                // Add position point for each coordinate
                int seqNo = 1;
                foreach (var coord in coords)
                {
                    var xmlPoint = mapContext.AddPositionPoint(null, seqNo, coord.X, coord.Y);
                    xmlPositionPoints.Add(xmlPoint);
                    seqNo++;
                }

                return new PropertyModification()
                {
                    Name = "positionPoints",
                    Value = new Equipment.PositionPoints() { PositionPoint = xmlPositionPoints.ToArray() }
                };
            }


            //throw new Exception("Error mapping " + valueObject.GetType().Name);
            return null;

        }


    }
}
