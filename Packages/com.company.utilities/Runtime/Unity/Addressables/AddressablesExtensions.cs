using UnityEngine.AddressableAssets;

namespace Company.Utilities.Runtime.Unity
{
    public static class AddressablesExtensions
    {
        public static void ReleaseAsset(this AssetReference assetReference)
        {
            if (assetReference.IsValid())
            {
                assetReference.ReleaseAsset();
            }
        }
    }
}