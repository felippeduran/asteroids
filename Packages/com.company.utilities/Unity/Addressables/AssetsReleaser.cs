using UnityEngine.AddressableAssets;
using Company.Utilities.Runtime;

namespace Company.Utilities.Unity
{
    public class AssetsReleaser : IAssetsReleaser
    {
        readonly AssetReference[] assetsReferences;

        public AssetsReleaser(params AssetReference[] assetsReferences)
        {
            this.assetsReferences = assetsReferences;
        }

        public void ReleaseAll()

        {
            foreach (var assetReference in assetsReferences)
            {
                assetReference.ReleaseAsset();
            }
        }
    }
}