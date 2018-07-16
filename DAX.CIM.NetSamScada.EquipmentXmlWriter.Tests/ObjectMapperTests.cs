using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DAX.CIM.NetSamScada.EquipmentXmlWriter.Mapping;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DAX.CIM.NetSamScada.PreProcessors;
using System.Xml.Serialization;
using DAX.CIM.PhysicalNetworkModel.Traversal;
using DAX.CIM.PhysicalNetworkModel;

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

            var mapper = new PNM2NetSamObjectMapper(_context);

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

            var xmlProfile = converter.GetXMLData(converter.GetCimObjects().ToList());


        }

        [TestMethod]
        public void MapEngumTest()
        {
            var converter = new NetSamEquipmentXMLConverter(_cimObjects, new List<IPreProcessor> { new AddMissingBayProcessor(), new DisconnectedLinkProcessor(), new EnsureACLSUniqueNames() });

            var xmlProfile = converter.GetXMLData(converter.GetCimObjects().ToList());

            XmlSerializer xmlSerializer = new XmlSerializer(xmlProfile.GetType());
            System.IO.StreamWriter file = new System.IO.StreamWriter(@"c:\temp\cim\engum_anonymized_net.xml");
            xmlSerializer.Serialize(file, xmlProfile);
            file.Close();

        }

        [TestMethod]
        public void CompleteNrgiTest()
        {
            bool run = true;

            if (run)
            {
                var reader = new CimJsonFileReader(@"C:\temp\cim\complete_net.jsonl");
                var cimObjects = reader.Read().ToList();

                Assert.IsTrue(CimObjUniquenessChecker.IsUnique(cimObjects));

                var converter = new NetSamEquipmentXMLConverter(cimObjects, new List<IPreProcessor> { new AddMissingBayProcessor(), new DisconnectedLinkProcessor(), new EnsureACLSUniqueNames() });

                var result = converter.GetCimObjects().ToList();
                Assert.IsTrue(CimObjUniquenessChecker.IsUnique(result));

                var xmlProfile = converter.GetXMLData(result);

                var disTest = cimObjects.Find(o => o.mRID == "b23c83e3-dc01-4748-acde-c19c80b934e2") as Disconnector;

                var disBayTest = result.Find(o => o.mRID == disTest.EquipmentContainer.@ref);

                // Check at DisconnectedLinkProcessor disconnectors er kommet med ud i xml
                Assert.IsTrue(xmlProfile.Disconnector.ToList().Exists(o => o.description == "Auto generated DL"));

                // Check at DisconnectedLinkProcessor bays er kommet med ud i xml
                Assert.IsTrue(xmlProfile.BayExt.ToList().Exists(o => o.description == "Auto generated DL Bay"));

                // Check at AddMissingBayProcessor bays er kommet med ud i xml
                Assert.IsTrue(xmlProfile.BayExt.ToList().Exists(o => o.mRID == disTest.EquipmentContainer.@ref));

                XmlSerializer xmlSerializer = new XmlSerializer(xmlProfile.GetType());
                System.IO.StreamWriter file = new System.IO.StreamWriter(@"c:\temp\cim\complete_net.xml");
                xmlSerializer.Serialize(file, xmlProfile);
                file.Close();
            }
        }

        [TestMethod]
        public void CompleteNrgiDeltaInitialDataSetTest()
        {
            bool run = true;

            if (run)
            {
                var reader = new CimJsonFileReader(@"c:\temp\cim\delta\00_initial 31-05-2018.jsonl");
                var cimObjects = reader.Read().ToList();

                Assert.IsTrue(CimObjUniquenessChecker.IsUnique(cimObjects));

                var converter = new NetSamEquipmentXMLConverter(cimObjects, new List<IPreProcessor> { new AddMissingBayProcessor(), new DisconnectedLinkProcessor(), new EnsureACLSUniqueNames() });

                var result = converter.GetCimObjects().ToList();
                Assert.IsTrue(CimObjUniquenessChecker.IsUnique(result));

                var xmlProfile = converter.GetXMLData(result);

                var disTest = cimObjects.Find(o => o.mRID == "b23c83e3-dc01-4748-acde-c19c80b934e2") as Disconnector;

                var disBayTest = result.Find(o => o.mRID == disTest.EquipmentContainer.@ref);

                // Check at DisconnectedLinkProcessor disconnectors er kommet med ud i xml
                Assert.IsTrue(xmlProfile.Disconnector.ToList().Exists(o => o.description == "Auto generated DL"));

                // Check at DisconnectedLinkProcessor bays er kommet med ud i xml
                Assert.IsTrue(xmlProfile.BayExt.ToList().Exists(o => o.description == "Auto generated DL Bay"));

                // Check at AddMissingBayProcessor bays er kommet med ud i xml
                Assert.IsTrue(xmlProfile.BayExt.ToList().Exists(o => o.mRID == disTest.EquipmentContainer.@ref));

                XmlSerializer xmlSerializer = new XmlSerializer(xmlProfile.GetType());
                System.IO.StreamWriter file = new System.IO.StreamWriter(@"c:\temp\cim\delta\00_initial 31-05-2018.xml");
                xmlSerializer.Serialize(file, xmlProfile);
                file.Close();
            }
        }

    }
}
