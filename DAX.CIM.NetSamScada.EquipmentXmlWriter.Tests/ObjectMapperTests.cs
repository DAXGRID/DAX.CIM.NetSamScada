using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DAX.CIM.NetSamScada.EquipmentXmlWriter.Mapping;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DAX.CIM.NetSamScada.PreProcessors;
using System.Xml.Serialization;
using DAX.CIM.PhysicalNetworkModel.Traversal;

namespace DAX.CIM.NetSamScada.EquipmentXmlWriter.Tests
{
    [TestClass]
    public class ObjectMapperTests : FixtureBase
    {
        CimContext _context;
        List<PhysicalNetworkModel.IdentifiedObject> _cimObjects;

        protected override void SetUp()
        {
            var reader = new CimJsonFileReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Data\engum_anonymized.jsonl"));
            _cimObjects = reader.Read().ToList();
            _context = CimContext.Create(_cimObjects);
            Using(_context);
        }

        [TestMethod]
        public void MapACLSTest()
        {
            MappingContext mapContext = new MappingContext();

            List<PhysicalNetworkModel.IdentifiedObject> cimObjects = new List<PhysicalNetworkModel.IdentifiedObject>();

            var aclsAsset = new PhysicalNetworkModel.Asset();
            aclsAsset.mRID = Guid.NewGuid().ToString();
            aclsAsset.name = "ACLS Asset";
            cimObjects.Add(aclsAsset);

            var aclsLoc = new PhysicalNetworkModel.LocationExt();
            aclsLoc.mRID = Guid.NewGuid().ToString();
            aclsLoc.coordinates = new PhysicalNetworkModel.Point2D[] { new PhysicalNetworkModel.Point2D() { X = 1, Y = 2 }, new PhysicalNetworkModel.Point2D() { X = 3, Y = 4 } };
            cimObjects.Add(aclsLoc);

            var acls = new PhysicalNetworkModel.ACLineSegmentExt();
            acls.mRID = Guid.NewGuid().ToString();
            acls.name = "acls blah blah";
            acls.description = "acls blah blah";
            acls.x = new PhysicalNetworkModel.Reactance() { multiplier = PhysicalNetworkModel.UnitMultiplier.c, unit = PhysicalNetworkModel.UnitSymbol.A, Value = 9.9 };
            acls.x0 = new PhysicalNetworkModel.Reactance() { multiplier = PhysicalNetworkModel.UnitMultiplier.c, unit = PhysicalNetworkModel.UnitSymbol.A, Value = 0.9 };
            acls.r = new PhysicalNetworkModel.Resistance() { multiplier = PhysicalNetworkModel.UnitMultiplier.micro, unit = PhysicalNetworkModel.UnitSymbol.A, Value = 8.8 };
            acls.r0 = new PhysicalNetworkModel.Resistance() { multiplier = PhysicalNetworkModel.UnitMultiplier.micro, unit = PhysicalNetworkModel.UnitSymbol.A, Value = 0.8 };
            acls.c = new PhysicalNetworkModel.Capacitance() { multiplier = PhysicalNetworkModel.UnitMultiplier.k, unit = PhysicalNetworkModel.UnitSymbol.H, Value = 7.7 };
            acls.c0 = new PhysicalNetworkModel.Capacitance() { multiplier = PhysicalNetworkModel.UnitMultiplier.k, unit = PhysicalNetworkModel.UnitSymbol.H, Value = 0.7 };
            acls.bch = new PhysicalNetworkModel.Susceptance() { multiplier = PhysicalNetworkModel.UnitMultiplier.k, unit = PhysicalNetworkModel.UnitSymbol.H, Value = 6.6 };
            acls.b0ch = new PhysicalNetworkModel.Susceptance() { multiplier = PhysicalNetworkModel.UnitMultiplier.k, unit = PhysicalNetworkModel.UnitSymbol.H, Value = 0.6 };
            acls.gch = new PhysicalNetworkModel.Conductance() { multiplier = PhysicalNetworkModel.UnitMultiplier.k, unit = PhysicalNetworkModel.UnitSymbol.H, Value = 5.5 };
            acls.g0ch = new PhysicalNetworkModel.Conductance() { multiplier = PhysicalNetworkModel.UnitMultiplier.k, unit = PhysicalNetworkModel.UnitSymbol.H, Value = 5.5 };
            acls.length = new PhysicalNetworkModel.Length() { multiplier = PhysicalNetworkModel.UnitMultiplier.k, unit = PhysicalNetworkModel.UnitSymbol.m, Value = 100 };
            acls.maximumCurrent = new PhysicalNetworkModel.CurrentFlow() { multiplier = PhysicalNetworkModel.UnitMultiplier.k, unit = PhysicalNetworkModel.UnitSymbol.H, Value = 4.4 };
            acls.PSRType = "Cable";
            acls.BaseVoltage = 10000;

            // ACLS relations
            acls.Assets = new PhysicalNetworkModel.PowerSystemResourceAssets() { @ref = aclsAsset.mRID };
            acls.Location = new PhysicalNetworkModel.PowerSystemResourceLocation() { @ref = aclsLoc.mRID };
            cimObjects.Add(acls);

            var mapper = new PNM2NetSamObjectMapper();

            var netSamAcls = mapper.MapObject(mapContext, acls) as Equipment.ACLineSegment;
            var netSamAclsAsset = mapper.MapObject(mapContext, aclsAsset) as Equipment.Asset;
            var netSamAclsLoc = mapper.MapObject(mapContext, aclsLoc) as Equipment.Location;

            // Make sure base voltage and psr type objects has been created
            Assert.IsNotNull(netSamAcls.BaseVoltage.@ref);
            Assert.IsNotNull(netSamAcls.PSRType.@ref);

            // Check that position point with seq = 1 and x = 1 exists
            Assert.IsNotNull(mapContext.PositionPoints.Find(o => o.sequenceNumber == "1" && o.xPosition == "1"));

            // Check that position point with seq = 2 and y = 4 exists
            Assert.IsNotNull(mapContext.PositionPoints.Find(o => o.sequenceNumber == "2" && o.yPosition == "4"));

            // Try do an XML mapping on it
            var converter = new NetSamEquipmentXMLConverter(cimObjects);

            var xmlProfile = converter.GetXMLData();


        }

        [TestMethod]
        public void MapEngumTest()
        {
            var converter = new NetSamEquipmentXMLConverter(_cimObjects, new List<IPreProcessor> { new AddMissingBayProcessor(), new DisconnectedLinkProcessor() });

            var xmlProfile = converter.GetXMLData();

            XmlSerializer xmlSerializer = new XmlSerializer(xmlProfile.GetType());
            System.IO.StreamWriter file = new System.IO.StreamWriter(@"c:\temp\cim\ny_engum.xml");
            xmlSerializer.Serialize(file, xmlProfile);
            file.Close();

        }

        [TestMethod]
        public void CompleteNrgiTest()
        {
            bool run = false;

            if (run)
            {
                var reader = new CimJsonFileReader(@"C:\temp\cim\complete_net.jsonl");
                var cimObjects = reader.Read().ToList();

                Assert.IsTrue(CimObjUniquenessChecker.IsUnique(cimObjects));

                var converter = new NetSamEquipmentXMLConverter(cimObjects, new List<IPreProcessor> { new AddMissingBayProcessor(), new DisconnectedLinkProcessor() });

                var result = converter.GetCimObjects();
                Assert.IsTrue(CimObjUniquenessChecker.IsUnique(result));

                var xmlProfile = converter.GetXMLData();

                XmlSerializer xmlSerializer = new XmlSerializer(xmlProfile.GetType());
                System.IO.StreamWriter file = new System.IO.StreamWriter(@"c:\temp\cim\complete_net.xml");
                xmlSerializer.Serialize(file, xmlProfile);
                file.Close();
            }
        }

    }
}
