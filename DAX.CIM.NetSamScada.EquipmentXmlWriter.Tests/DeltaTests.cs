using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using DAX.CIM.Differ;
using DAX.CIM.NetSamScada.AssetXmlWriter;
using DAX.CIM.NetSamScada.AssetXmlWriter.Diff;
using DAX.CIM.NetSamScada.AssetXmlWriter.Mapping;
using DAX.CIM.NetSamScada.Delta;
using DAX.CIM.NetSamScada.EquipmentXmlWriter.Diff;
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
                deltaFilesPath + "18_change_substation_by_attribute_product_type_ec.jsonl",
                deltaFilesPath + "19_initial_export_before_HV_changes.jsonl",
                deltaFilesPath + "20_change_HV_trafo_by_attributes.jsonl",
                deltaFilesPath + "21_change_HV_trafo_by_delete_and_create.jsonl",
                deltaFilesPath + "22_change_HV_cable_by_attributes.jsonl",
                deltaFilesPath + "23_change_HV_cable_by_delete_and_create.jsonl",
                deltaFilesPath + "24_add_new_HV_bay.jsonl"
            };

            for (int diffStep = 5; diffStep < 6; diffStep++)
            {
                CreateDiffXmls(diffFiles, diffStep);
            }
        }


        [TestMethod]
        public void TestSingleDiff()
        {
            var deltaFilesPath = @"c:\temp\cim\delta\";

            var deltaFile1 = deltaFilesPath + "04_insert_LV_bay_with_fuse_switch.jsonl";
            var deltaFile2 = deltaFilesPath + "05_insert_new_LV_cable.jsonl";


            var eqCimObjects = EquipmentDeltaDiffer.CreateEquipmentDeltaXML(deltaFile1, deltaFile2, "test-delta.jsonl");

            AssetDeltaDiffer.CreateAssetDeltaXML(deltaFile1, deltaFile2, "test-delta-asset.jsonl", eqCimObjects);

        }


        private static void CreateDiffXmls(string[] diffFiles, int diffStep)
        {
            var diff1 = diffFiles[diffStep - 1];
            var diff2 = diffFiles[diffStep];
            var outputFilename = diff2.Replace("jsonl", "xml");

            EquipmentDeltaDiffer.CreateEquipmentDeltaXML(diff1, diff2, outputFilename);
        }

      
    }
}
