using DAX.IO.CIM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DAX.CIM.NetSamScada.AssetXmlWriter.Mapping
{
    public class AssetMappingContext
    {
        public Dictionary<string, string> AssetToPsrRefs = new Dictionary<string, string>();

        private Guid _ownerGuidBase = Guid.Parse("3f713525-6ad4-4517-aea4-c2aaf78e0f31");
        private Dictionary<string, Asset.AssetOwner> _owners = new Dictionary<string, Asset.AssetOwner>();

        private Guid _maintainerGuidBase = Guid.Parse("1243a8e0-3f5f-4c7e-bf22-c3fa32e2eeb0");
        private Dictionary<string, Asset.Maintainer> _maintainers = new Dictionary<string, Asset.Maintainer>();

        public List<Asset.AssetOwner> AssetOwners { get { return _owners.Values.ToList(); } }
        public List<Asset.Maintainer> AssetMaintainers { get { return _maintainers.Values.ToList(); } }

        public AssetMappingContext(List<PhysicalNetworkModel.IdentifiedObject> cimObjects)
        {
            // createa asset to psr lookup table
            foreach (var cimObj in cimObjects)
            {
                if (cimObj is PhysicalNetworkModel.PowerSystemResource)
                {
                    var psr = cimObj as PhysicalNetworkModel.PowerSystemResource;

                    if (psr.Assets != null && psr.Assets.@ref != null)
                       AssetToPsrRefs.Add(psr.Assets.@ref, psr.mRID);
                }
            }
        }

        public Asset.AssetOwner GetOrCreateAssetOwnerByName(string name)
        {
            if (!_owners.ContainsKey(name))
            {
                var assetOwner = new Asset.AssetOwner();
                assetOwner.name = name;
                //assetOwner.mRID = GUIDHelper.CreateDerivedGuid(_ownerGuidBase, _owners.Count + 1).ToString();
                assetOwner.mRID = StringToGUID("asset owner: " + name).ToString();
                _owners.Add(name, assetOwner);

                return assetOwner;
            }
            else
                return _owners[name];
        }

        public Asset.Maintainer GetOrCreateAssetMaintainerByName(string name)
        {
            if (!_maintainers.ContainsKey(name))
            {
                var maintainer = new Asset.Maintainer();
                maintainer.name = name;
                //maintainer.mRID = GUIDHelper.CreateDerivedGuid(_maintainerGuidBase, _maintainers.Count + 1).ToString();
                maintainer.mRID = StringToGUID("asset maintainer: " + name).ToString();
                _maintainers.Add(name, maintainer);

                return maintainer;
            }
            else
                return _maintainers[name];
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
