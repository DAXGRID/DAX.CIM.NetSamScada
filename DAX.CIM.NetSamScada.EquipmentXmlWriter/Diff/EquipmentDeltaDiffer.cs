using DAX.CIM.NetSamScada.PreProcessors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAX.CIM.NetSamScada.Delta;
using DAX.CIM.NetSamScada.EquipmentXmlWriter.Mapping;
using DAX.Cson;
using DAX.CIM.Differ;
using System.Xml.Serialization;

namespace DAX.CIM.NetSamScada.EquipmentXmlWriter.Diff
{
    public static class EquipmentDeltaDiffer
    {
        public static List<PhysicalNetworkModel.IdentifiedObject> CreateEquipmentDeltaXML(string diff1, string diff2, string outputFilename)
        {
            var differ = new CimDiffer();
            var serializer = new CsonSerializer();

            var prevFile = serializer.DeserializeObjects(File.OpenRead(diff1)).ToList();
            var prevConverter = new NetSamEquipmentXMLConverter(prevFile, new List<IPreProcessor> { new AddMissingBayProcessor(), new DisconnectedLinkProcessor(), new EnsureACLSUniqueNames() });
            var prevResult = prevConverter.GetCimObjects().ToList();

            var nextFile = serializer.DeserializeObjects(File.OpenRead(diff2)).ToList();
            var nextConverter = new NetSamEquipmentXMLConverter(nextFile, new List<IPreProcessor> { new AddMissingBayProcessor(), new DisconnectedLinkProcessor(), new EnsureACLSUniqueNames() });
            var nextResult = nextConverter.GetCimObjects().ToList();


            var diffObjs = differ.GetDiff(prevResult, nextResult).ToList();


            var diffSerializer = new CsonSerializer();


            using (var destination = File.Open("diff.jsonl", FileMode.Create))
            {
                using (var source = diffSerializer.SerializeObjects(diffObjs))
                {
                    source.CopyTo(destination);
                }
            }


            var objectCreations = diffObjs.Where(o => o.Change is DAX.CIM.PhysicalNetworkModel.Changes.ObjectCreation).ToList();


            var env = new Delta.Equipment.ChangeSet();

            List<Delta.Equipment.ObjectCreation> xmlObjectCreations = new List<Delta.Equipment.ObjectCreation>();

            var mapContext = new DAX.CIM.NetSamScada.EquipmentXmlWriter.Mapping.MappingContext();
            PNM2NetSamObjectMapper mapper = new PNM2NetSamObjectMapper(null);

            foreach (var objectCreation in objectCreations)
            {
                DAX.CIM.PhysicalNetworkModel.Changes.ObjectCreation change = objectCreation.Change as DAX.CIM.PhysicalNetworkModel.Changes.ObjectCreation;
                var xmlObj = mapper.MapObject(mapContext, change.Object);

                if (xmlObj != null)
                    xmlObjectCreations.Add(new Delta.Equipment.ObjectCreation() { Object = xmlObj, referenceType = objectCreation.TargetObject.referenceType, @ref = objectCreation.TargetObject.@ref });

            }


            env.ObjectCreation = xmlObjectCreations.ToArray();

            /////////////////////////////
            /// modifications
            List<Delta.Equipment.ObjectModification> xmlObjectModifications = new List<Delta.Equipment.ObjectModification>();

            var objectModifications = diffObjs.Where(o => o.Change is DAX.CIM.PhysicalNetworkModel.Changes.ObjectModification).ToList();

            foreach (var objectModification in objectModifications)
            {
                DAX.CIM.PhysicalNetworkModel.Changes.ObjectModification change = objectModification.Change as DAX.CIM.PhysicalNetworkModel.Changes.ObjectModification;

                // Add forward changes
                var xmlObjectModification = new Delta.Equipment.ObjectModification() { referenceType = objectModification.TargetObject.referenceType, @ref = objectModification.TargetObject.@ref };

                List<Delta.Equipment.PropertyModification> xmlPropertyModifications = new List<Delta.Equipment.PropertyModification>();

                foreach (var propModification in change.Modifications)
                {
                    var xmlPropModification = mapper.MapProperty(mapContext, propModification.Name, propModification.Value);
                    xmlPropertyModifications.Add(xmlPropModification);
                }

                xmlObjectModification.ForwardChange = xmlPropertyModifications.ToArray();

                // Add reverse changes
                DAX.CIM.PhysicalNetworkModel.Changes.ObjectReverseModification reverseChange = objectModification.ReverseChange as DAX.CIM.PhysicalNetworkModel.Changes.ObjectReverseModification;

                List<Delta.Equipment.PropertyModification> xmlPropertyRevserseModifications = new List<Delta.Equipment.PropertyModification>();

                foreach (var propModification in reverseChange.Modifications)
                {
                    var xmlPropModification = mapper.MapProperty(mapContext, propModification.Name, propModification.Value);
                    xmlPropertyRevserseModifications.Add(xmlPropModification);
                }

                xmlObjectModification.ReverseChange = new Delta.Equipment.ObjectReverseModification() { Property = xmlPropertyRevserseModifications.ToArray() };


                xmlObjectModifications.Add(xmlObjectModification);
            }

            env.ObjectModification = xmlObjectModifications.ToArray();

            ////////////////////////////////////////
            // Deletion

            List<Delta.Equipment.ObjectDeletion> xmlObjectDeletions = new List<Delta.Equipment.ObjectDeletion>();

            var objectDeletions = diffObjs.Where(o => o.Change is DAX.CIM.PhysicalNetworkModel.Changes.ObjectDeletion).ToList();
            foreach (var objectDeletion in objectDeletions)
            {
                var xmlObjectDeletion = new Delta.Equipment.ObjectDeletion() { referenceType = objectDeletion.TargetObject.referenceType, @ref = objectDeletion.TargetObject.@ref };
                xmlObjectDeletions.Add(xmlObjectDeletion);
            }

            env.ObjectDeletion = xmlObjectDeletions.ToArray();




            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("xs", "http://www.w3.org/2001/XMLSchema");
            ns.Add("cim", "http://visue.dk/equipment_2_0");
            ns.Add("delta", "http://visue.dk/equipment_delta_2_0.xsd");
            ns.Add("instance", "http://www.w3.org/2001/XMLSchema-instance");


            XmlSerializer xmlSerializer = new XmlSerializer(env.GetType());
            System.IO.StreamWriter file = new System.IO.StreamWriter(outputFilename);
            xmlSerializer.Serialize(file, env, ns);
            file.Close();

            return nextResult;
        }
    }
}
