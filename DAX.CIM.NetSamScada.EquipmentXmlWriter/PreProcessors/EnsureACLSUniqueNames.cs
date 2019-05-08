using DAX.CIM.PhysicalNetworkModel;
using DAX.CIM.PhysicalNetworkModel.Traversal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAX.CIM.NetSamScada.PreProcessors
{
    /// <summary>
    /// Used to satisfy PSI SCADA - ensuring that low voltage ACLS's have a unique name
    /// </summary>
    public class EnsureACLSUniqueNames : IPreProcessor
    {
        public IEnumerable<IdentifiedObject> Transform(CimContext context, IEnumerable<IdentifiedObject> input)
        {
            List<IdentifiedObject> output = new List<IdentifiedObject>();

            List<ACLineSegment> aclsToCheck = new List<ACLineSegment>();

            foreach (var inputCimObject in input)
            {
                if (inputCimObject is ACLineSegment && ((ACLineSegment)inputCimObject).BaseVoltage == 400)
                    aclsToCheck.Add(inputCimObject as ACLineSegment);

                output.Add(inputCimObject);
            }


            var results = from c in aclsToCheck
                          group c by c.name into g
                          select new { Name = g.Key, Duplicates = g.ToList() };

            foreach (var r in results)
            {
                if (r.Duplicates.Count > 1)
                {
                    if (r.Name == null)
                    {
                        // put in part of mRID
                        foreach (var d in r.Duplicates)
                        {
                            d.name = d.mRID.Substring(d.mRID.Length - 16, 16);
                        }
                    }
                    else
                    {
                        int counter = 1;
                        foreach (var d in r.Duplicates)
                        {
                            d.name = d.mRID;
                            counter++;
                        }
                    }
                }
            }

            // Check that we're really unique
            var checkResults = from c in aclsToCheck
                               group c by c.name into g
                               select new { Name = g.Key, Duplicates = g.ToList() };

            if (checkResults.ToArray().Length != aclsToCheck.Count)
                throw new Exception("Someting rotten. Low voltage ACLS still not unique");

            foreach (var cimObj in output)
                yield return cimObj;
        }
    }
}
