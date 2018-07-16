using DAX.IO.CIM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAX.CIM.NetSamScada.EquipmentXmlWriter.Mapping
{
    public class MappingContext
    {
        private Guid _baseVoltageGuidBase = Guid.Parse("bd98ed53-7e5f-4822-bdbb-aab9325b6185");
        private Dictionary<double, Equipment.BaseVoltage> _baseVoltages = new Dictionary<double, Equipment.BaseVoltage>();

        private Guid _psrTypeGuidBase = Guid.Parse("5c27dde5-001c-44d1-8d08-149130ef94fd");
        private Dictionary<string, Equipment.PSRType> _psrTypes = new Dictionary<string, Equipment.PSRType>();

        private Guid _ownerGuidBase = Guid.Parse("3f713525-6ad4-4517-aea4-c2aaf78e0f31");
        private Dictionary<string, Equipment.AssetOwner> _owners = new Dictionary<string, Equipment.AssetOwner>();

        private Guid _maintainerGuidBase = Guid.Parse("1243a8e0-3f5f-4c7e-bf22-c3fa32e2eeb0");
        private Dictionary<string, Equipment.Maintainer> _maintainers = new Dictionary<string, Equipment.Maintainer>();

        private List<Equipment.PositionPoint> _positionPoints = new List<Equipment.PositionPoint>();

        public List<Equipment.PositionPoint> PositionPoints { get { return _positionPoints; } }
        public List<Equipment.PSRType> PSRTypes { get { return _psrTypes.Values.ToList(); } }
        public List<Equipment.BaseVoltage> BaseVoltages { get { return _baseVoltages.Values.ToList(); } }
        public List<Equipment.AssetOwner> AssetOwners { get { return _owners.Values.ToList(); } }
        public List<Equipment.Maintainer> AssetMaintainers { get { return _maintainers.Values.ToList(); } }

        public Equipment.BaseVoltage GetOrCreateBaseVoltage(double voltageLevel)
        {
            if (!_baseVoltages.ContainsKey(voltageLevel))
            {
                var newBaseVoltage = new Equipment.BaseVoltage();
                newBaseVoltage.nominalVoltage = new Equipment.Voltage() { multiplier = Equipment.UnitMultiplier.none, unit = Equipment.UnitSymbol.V, unitSpecified = true, Value = Convert.ToSingle(voltageLevel) };
                newBaseVoltage.mRID = GUIDHelper.CreateDerivedGuid(_baseVoltageGuidBase, _baseVoltages.Count + 1).ToString();
                _baseVoltages.Add(voltageLevel, newBaseVoltage);

                return newBaseVoltage;
            }
            else
                return _baseVoltages[voltageLevel];
        }

        public Equipment.PSRType GetOrCreatePSRTypeByName(string name)
        {
            if (!_psrTypes.ContainsKey(name))
            {
                var newPsrType = new Equipment.PSRType();
                newPsrType.name = name;
                newPsrType.mRID = GUIDHelper.CreateDerivedGuid(_psrTypeGuidBase, _psrTypes.Count + 1).ToString();
                _psrTypes.Add(name, newPsrType);

                return newPsrType;
            }
            else
                return _psrTypes[name];
        }

        public Equipment.AssetOwner GetOrCreateAssetOwnerByName(string name)
        {
            if (!_owners.ContainsKey(name))
            {
                var assetOwner = new Equipment.AssetOwner();
                assetOwner.name = name;
                assetOwner.mRID = GUIDHelper.CreateDerivedGuid(_ownerGuidBase, _owners.Count + 1).ToString();
                _owners.Add(name, assetOwner);

                return assetOwner;
            }
            else
                return _owners[name];
        }

        public Equipment.Maintainer GetOrCreateAssetMaintainerByName(string name)
        {
            if (!_maintainers.ContainsKey(name))
            {
                var maintainer = new Equipment.Maintainer();
                maintainer.name = name;
                maintainer.mRID = GUIDHelper.CreateDerivedGuid(_maintainerGuidBase, _maintainers.Count + 1).ToString();
                _maintainers.Add(name, maintainer);

                return maintainer;
            }
            else
                return _maintainers[name];
        }

        public Equipment.PositionPoint AddPositionPoint(string locationMrid, int seqNo, double x, double y)
        {
            var pp = new Equipment.PositionPoint();
            if (locationMrid != null)
                pp.Location = new Equipment.PositionPointLocation() { @ref = locationMrid };
            pp.sequenceNumber = seqNo.ToString();
            pp.xPosition = Convert.ToString(x, System.Globalization.CultureInfo.InvariantCulture);
            pp.yPosition = Convert.ToString(y, System.Globalization.CultureInfo.InvariantCulture);
            _positionPoints.Add(pp);
            return pp;
        }
    }
}
