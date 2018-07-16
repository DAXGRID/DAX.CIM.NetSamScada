using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using DAX.CIM.Differ;
using DAX.CIM.NetSamScada.Delta;
using DAX.CIM.NetSamScada.EquipmentXmlWriter.Mapping;
using DAX.CIM.NetSamScada.PreProcessors;
using DAX.Cson;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DAX.CIM.NetSamScada.EquipmentXmlWriter.Tests
{
    [TestClass]
    public class DeltaTests
    {

        [TestMethod]
        public void TestMethod1()
        {

            var deltaFilesPath = @"c:\temp\cim\delta\";

            /*
            // big files
            var diffBigFiles = new string[] {
                deltaFilesPath + "00_initial 31-05-2018.jsonl",
                deltaFilesPath + "01_LV_ACLineSegmentmoved from trafo to bay 31-05-2018.jsonl",
                deltaFilesPath + "02_MV_ACLineSegment Change Type 31-05-2018.jsonl",
            };

            for (int diffStep = 1; diffStep < 3; diffStep++)
            {
                CreateDiffXmls(diffBigFiles, diffStep);
            }
            */


            // small files
            var diffFiles = new string[] {
                deltaFilesPath + "00_",
                deltaFilesPath + "01_",
                deltaFilesPath + "02_small_area.jsonl",
                deltaFilesPath + "03_change_LV_switch_status.jsonl",
                deltaFilesPath + "04_insert_LV_bay_with_fuse_switch.jsonl",
                deltaFilesPath + "05_insert_new_LV_cable.jsonl",
                deltaFilesPath + "06_insert_new_LV_cablebox.jsonl",
                deltaFilesPath + "07_insert_LV_customer_bay_in_cablebox.jsonl",
                deltaFilesPath + "08_insert_LV_customer_cable_and_installation.jsonl",
                deltaFilesPath + "09_insert_MV_station_888_split_MV_cable_connect_to_new_station.jsonl",
                deltaFilesPath + "10_cablebox_changed_product_type_date_attributes.jsonl",
                deltaFilesPath + "11_delete_kablebox_LV_cable_to_Substation_131.jsonl",
                deltaFilesPath + "12_delete_customers_cable.jsonl",
                deltaFilesPath + "13_change_customer.jsonl",
                deltaFilesPath + "14_delete_substation_delete_MVcable_new_MV_cable.jsonl",
                deltaFilesPath + "15_change_asset_attributes.jsonl",
                deltaFilesPath + "16_change_MV_trafo_by_attributes.jsonl",
                deltaFilesPath + "17_change_MV_trafo_by_delete_and_create.jsonl",
                deltaFilesPath + "18_change_substation_by_attribute_product_type_ec.jsonl"
            };

            for (int diffStep = 3; diffStep < 19; diffStep++)
            {
                CreateDiffXmls(diffFiles, diffStep);
            }



        }

        private static void CreateDiffXmls(string[] diffFiles, int diffStep)
        {
            var differ = new CimDiffer();
            var serializer = new CsonSerializer();

            var prevFile = serializer.DeserializeObjects(File.OpenRead(diffFiles[diffStep - 1])).ToList();
            var prevConverter = new NetSamEquipmentXMLConverter(prevFile, new List<IPreProcessor> { new AddMissingBayProcessor(), new DisconnectedLinkProcessor(), new EnsureACLSUniqueNames() });
            var prevResult = prevConverter.GetCimObjects().ToList();

            var nextFile = serializer.DeserializeObjects(File.OpenRead(diffFiles[diffStep])).ToList();
            var nextConverter = new NetSamEquipmentXMLConverter(nextFile, new List<IPreProcessor> { new AddMissingBayProcessor(), new DisconnectedLinkProcessor(), new EnsureACLSUniqueNames() });
            var nextResult = nextConverter.GetCimObjects().ToList();


            var diffObjs = differ.GetDiff(prevResult, nextResult).ToList();

            var objectCreations = diffObjs.Where(o => o.Change is DAX.CIM.PhysicalNetworkModel.Changes.ObjectCreation).ToList();


            NetSamScada.Delta.ChangeSet env = new Delta.ChangeSet();

            List<ObjectCreation> xmlObjectCreations = new List<ObjectCreation>();

            var mapContext = new DAX.CIM.NetSamScada.EquipmentXmlWriter.Mapping.MappingContext();
            PNM2NetSamObjectMapper mapper = new PNM2NetSamObjectMapper(null);

            foreach (var objectCreation in objectCreations)
            {
                DAX.CIM.PhysicalNetworkModel.Changes.ObjectCreation change = objectCreation.Change as DAX.CIM.PhysicalNetworkModel.Changes.ObjectCreation;
                var xmlObj = mapper.MapObject(mapContext, change.Object);
                xmlObjectCreations.Add(new ObjectCreation() { Object = xmlObj, referenceType = objectCreation.TargetObject.referenceType, @ref = objectCreation.TargetObject.@ref });

            }





            env.ObjectCreation = xmlObjectCreations.ToArray();

            /////////////////////////////
            /// modifications
            List<ObjectModification> xmlObjectModifications = new List<ObjectModification>();

            var objectModifications = diffObjs.Where(o => o.Change is DAX.CIM.PhysicalNetworkModel.Changes.ObjectModification).ToList();

            foreach (var objectModification in objectModifications)
            {
                DAX.CIM.PhysicalNetworkModel.Changes.ObjectModification change = objectModification.Change as DAX.CIM.PhysicalNetworkModel.Changes.ObjectModification;

                // Add forward changes
                var xmlObjectModification = new ObjectModification() { referenceType = objectModification.TargetObject.referenceType, @ref = objectModification.TargetObject.@ref };

                List<PropertyModification> xmlPropertyModifications = new List<PropertyModification>();

                foreach (var propModification in change.Modifications)
                {
                    var xmlPropModification = mapper.MapProperty(mapContext, propModification.Name, propModification.Value);
                    xmlPropertyModifications.Add(xmlPropModification);
                }

                xmlObjectModification.ForwardChange = xmlPropertyModifications.ToArray();

                // Add reverse changes
                DAX.CIM.PhysicalNetworkModel.Changes.ObjectReverseModification reverseChange = objectModification.ReverseChange as DAX.CIM.PhysicalNetworkModel.Changes.ObjectReverseModification;

                List<PropertyModification> xmlPropertyRevserseModifications = new List<PropertyModification>();

                foreach (var propModification in reverseChange.Modifications)
                {
                    var xmlPropModification = mapper.MapProperty(mapContext, propModification.Name, propModification.Value);
                    xmlPropertyRevserseModifications.Add(xmlPropModification);
                }

                xmlObjectModification.ReverseChange = new ObjectReverseModification() { Property = xmlPropertyRevserseModifications.ToArray() };


                xmlObjectModifications.Add(xmlObjectModification);
            }

            env.ObjectModification = xmlObjectModifications.ToArray();

            ////////////////////////////////////////
            // Deletion

            List<ObjectDeletion> xmlObjectDeletions = new List<ObjectDeletion>();

            var objectDeletions = diffObjs.Where(o => o.Change is DAX.CIM.PhysicalNetworkModel.Changes.ObjectDeletion).ToList();
            foreach (var objectDeletion in objectDeletions)
            {
                var xmlObjectDeletion = new ObjectDeletion() { referenceType = objectDeletion.TargetObject.referenceType, @ref = objectDeletion.TargetObject.@ref };
                xmlObjectDeletions.Add(xmlObjectDeletion);
            }

            env.ObjectDeletion = xmlObjectDeletions.ToArray();




            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("xs", "http://www.w3.org/2001/XMLSchema");
            ns.Add("cim", "http://net-sam.dk/EquipmentProfile_1_4");
            ns.Add("delta", "http://net-sam.dk/NetSamDeltaProfile_0_1.xsd");
            ns.Add("instance", "http://www.w3.org/2001/XMLSchema-instance");



            XmlSerializer xmlSerializer = new XmlSerializer(env.GetType());
            System.IO.StreamWriter file = new System.IO.StreamWriter(diffFiles[diffStep].Replace("jsonl", "xml"));
            xmlSerializer.Serialize(file, env, ns);
            file.Close();
        }
    }
}
