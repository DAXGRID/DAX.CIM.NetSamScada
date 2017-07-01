using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DAX.CIM.PhysicalNetworkModel;
using DAX.CIM.PhysicalNetworkModel.Traversal;
using DAX.CIM.PhysicalNetworkModel.Traversal.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DAX.CIM.NetSamScada.PreProcessors;

namespace DAX.CIM.NetSamScada.EquipmentXmlWriter.Tests
{
    [TestClass]
    public class DisconnectedLinkTests : FixtureBase
    {
        CimContext context;
        List<IdentifiedObject> cimObjects;

        protected override void SetUp()
        {
            // This station has both MV and LV cables connected directly to the power transformer
            var reader = new CimJsonFileReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Data\31081_dislink.jsonl"));
            cimObjects = reader.Read().ToList();
            context = CimContext.Create(cimObjects);
            Using(context);
        }

        [TestMethod]
        public void TestDisconnectedLinkSt31081()
        {
            var convert = new NetSamEquipmentXMLConverter(cimObjects, new List<IPreProcessor> { new DisconnectedLinkProcessor() });
            var result = convert.GetCimObjects().ToList();

            context = CimContext.Create(result);

            // Find power transformer 1 in 31081
            var pt1 = result.Find(o => o.mRID == "926c6c83-3ce5-4ac4-b9f1-a47ead9a21f8") as PowerTransformer;
            Assert.IsNotNull(pt1);

            // Get neighbor conduction equipmenents of the power transformer
            var tfNeighbors = pt1.GetNeighborConductingEquipments();

            // Expect two disconnectors
            Assert.AreEqual(2, tfNeighbors.Count(o => o is Disconnector));

            foreach (var sw in tfNeighbors)
            {
                // Check that sw has psrtype disconnected link
                Assert.AreEqual("DisconnectingLink", sw.PSRType);

                // Check that sw has ACLS neighbor
                Assert.IsNotNull(sw.GetNeighborConductingEquipments().Find(o => o is ACLineSegment));

                // Check that sw has a bay
                Assert.IsNotNull(sw.GetBay());

                // Check that sw has a substation equal to the pt
                Assert.AreEqual(pt1.GetSubstation(), sw.GetSubstation());
            }

            Assert.IsTrue(CimObjUniquenessChecker.IsUnique(result));
        }

        [TestMethod]
        public void TestDisconnectedLinkSt2079()
        {
            // This station has two lv cables connected to the same power transformer
            var reader = new CimJsonFileReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Data\2079_dislink.jsonl"));
            var cimObjects = reader.Read().ToList();

            var convert = new NetSamEquipmentXMLConverter(cimObjects, new List<IPreProcessor> { new DisconnectedLinkProcessor() });
            var result = convert.GetCimObjects().ToList();

            context = CimContext.Create(result);

            // Find power transformer 1 in 31081
            var pt1 = result.Find(o => o.mRID == "aee93781-4be6-4a56-b963-2a447d2d09e2") as PowerTransformer;
            Assert.IsNotNull(pt1);

            Assert.IsTrue(CimObjUniquenessChecker.IsUnique(result));
        }
    }
}
