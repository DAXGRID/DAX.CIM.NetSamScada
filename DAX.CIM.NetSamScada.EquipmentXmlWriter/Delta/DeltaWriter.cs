using DAX.CIM.Differ;
using DAX.CIM.NetSamScada.Delta;
using DAX.CIM.NetSamScada.EquipmentXmlWriter.Mapping;
using DAX.CIM.NetSamScada.PreProcessors;
using DAX.Cson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DAX.CIM.NetSamScada.EquipmentXmlWriter.Delta
{
    public class DeltaWriter
    {
        public static void WriteDeltaXML(string diff1Jsonl, string diff2Jsonl, string outputFilename)
        {
            var differ = new CimDiffer();
            var serializer = new CsonSerializer();

            var prevFile = serializer.DeserializeObjects(File.OpenRead(diff1Jsonl)).ToList();
            var prevConverter = new NetSamEquipmentXMLConverter(prevFile, new List<IPreProcessor> { new AddMissingBayProcessor(), new DisconnectedLinkProcessor(), new EnsureACLSUniqueNames() });
            var prevResult = prevConverter.GetCimObjects().ToList();

            var nextFile = serializer.DeserializeObjects(File.OpenRead(diff2Jsonl)).ToList();
            var nextConverter = new NetSamEquipmentXMLConverter(nextFile, new List<IPreProcessor> { new AddMissingBayProcessor(), new DisconnectedLinkProcessor(), new EnsureACLSUniqueNames() });
            var nextResult = nextConverter.GetCimObjects().ToList();


            var diffObjs = differ.GetDiff(prevResult, nextResult).ToList();

            var objectCreations = diffObjs.Where(o => o.Change is DAX.CIM.PhysicalNetworkModel.Changes.ObjectCreation).ToList();


            NetSamScada.Delta.ChangeSet env = new ChangeSet();

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
            System.IO.StreamWriter file = new System.IO.StreamWriter(outputFilename);
            xmlSerializer.Serialize(file, env, ns);
            file.Close();
        }

    }
}
