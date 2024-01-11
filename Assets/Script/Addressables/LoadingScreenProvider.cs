using UnityEngine;
using System.Threading.Tasks;

namespace Core.Loading
{
    public class LoadingScreenProvider : LocalAssetLoader
    {
        public Task<LoadingScreen> Load()
        {
            return LoadInternal<LoadingScreen>(("LoadingScreen"));
        }

        public void Unload()
        {
            UnloadInternal();
        }
    }
}