using DAX.IO.CIM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

        private List<Equipment.PositionPoint> _positionPoints = new List<Equipment.PositionPoint>();

        public List<Equipment.PositionPoint> PositionPoints { get { return _positionPoints; } }
        public List<Equipment.PSRType> PSRTypes { get { return _psrTypes.Values.ToList(); } }
        public List<Equipment.BaseVoltage> BaseVoltages { get { return _baseVoltages.Values.ToList(); } }
      
        public Equipment.BaseVoltage GetOrCreateBaseVoltage(double voltageLevel)
        {
            if (!_baseVoltages.ContainsKey(voltageLevel))
            {
                var newBaseVoltage = new Equipment.BaseVoltage();
                newBaseVoltage.nominalVoltage = new Equipment.Voltage() { multiplier = Equipment.UnitMultiplier.none, unit = Equipment.UnitSymbol.V, unitSpecified = true, Value = Convert.ToSingle(voltageLevel) };
                //newBaseVoltage.mRID = GUIDHelper.CreateDerivedGuid(_baseVoltageGuidBase, _baseVoltages.Count + 1).ToString();
                newBaseVoltage.mRID = StringToGUID("base voltage " + voltageLevel).ToString();

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
                //newPsrType.mRID = GUIDHelper.CreateDerivedGuid(_psrTypeGuidBase, _psrTypes.Count + 1).ToString();
                newPsrType.mRID = StringToGUID("psrtype " + name).ToString();
                _psrTypes.Add(name, newPsrType);

                return newPsrType;
            }
            else
                return _psrTypes[name];
        }


        public Equipment.PositionPoint AddPositionPoint(string locationMrid, int seqNo, double x, double y)
        {
            var pp = new Equipment.PositionPoint();
            pp.Location = new Equipment.PositionPointLocation() { @ref = locationMrid };
            pp.sequenceNumber = seqNo.ToString();
            pp.xPosition = Convert.ToString(x, System.Globalization.CultureInfo.InvariantCulture);
            pp.yPosition = Convert.ToString(y, System.Globalization.CultureInfo.InvariantCulture);
            _positionPoints.Add(pp);
            return pp;
        }

        private Guid StringToGUID(string value)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5 md5Hasher = MD5.Create();
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(value));
            return new Guid(data);
        }
    }
}
