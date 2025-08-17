using UnityEngine.AddressableAssets;

namespace Company.Utilities.Runtime
{
    public interface IAssetsReleaser
    {
        void ReleaseAll();
    }

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