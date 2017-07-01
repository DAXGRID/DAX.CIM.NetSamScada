using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using DAX.CIM.PhysicalNetworkModel.Traversal;
using DAX.CIM.PhysicalNetworkModel;
using System.Collections.Generic;
using DAX.CIM.NetSamScada.PreProcessors;

namespace DAX.CIM.NetSamScada.EquipmentXmlWriter.Tests
{
    [TestClass]
    public class AddMissingBayTests : FixtureBase
    {
        CimContext _context;
        List<IdentifiedObject> _cimObjects;

        protected override void SetUp()
        {
            var reader = new CimJsonFileReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Data\brb.jsonl"));
            _cimObjects = reader.Read().ToList();
            _context = CimContext.Create(_cimObjects);
            Using(_context);
        }

        [TestMethod]
        public void NetSamNoProcessorTest()
        {
           
            var convert = new NetSamEquipmentXMLConverter(_cimObjects);

            var result = convert.GetCimObjects().ToList();

            // No transformations, so we should get the same number of objects in the output
            Assert.AreEqual(_cimObjects.Count, result.Count);
        }

        [TestMethod]
        public void NetSamAddBayProcessorTest()
        {
            // We process substation BRB which has at least one switch that don't sit in a bay
            var convert = new NetSamEquipmentXMLConverter(_cimObjects, new List<IPreProcessor> { new AddMissingBayProcessor() });
            var result = convert.GetCimObjects().ToList();

            // Check that extra bays are created
            int nBaysInput = _cimObjects.Count(i => i is Bay);
            int nBaysOutput = result.Count(i => i is Bay);
            Assert.IsTrue(nBaysOutput > nBaysInput);

            // Get switch between the two MV busbars
            var sw = result.Find(o => o.mRID == "11b75495-ed32-41fd-863e-cf95ddbff563") as Switch;
            Assert.IsNotNull(sw);

            // Check that switch is pointing a bay now
            var swBay = result.Find(o => o.mRID == sw.EquipmentContainer.@ref) as Bay;
            Assert.IsNotNull(swBay);

            // Check that the fictional bay is pointing to correct voltage level container
            var bayVl = result.Find(o => o.mRID == swBay.VoltageLevel.@ref) as VoltageLevel;
            Assert.IsNotNull(bayVl);
            Assert.IsNotNull(bayVl.BaseVoltage == sw.BaseVoltage);

            Assert.IsTrue(CimObjUniquenessChecker.IsUnique(result));

        }
    }
}
