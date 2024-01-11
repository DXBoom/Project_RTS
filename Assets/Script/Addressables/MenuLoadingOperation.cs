using System;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Core.Loading
{
    public sealed class MenuLoadingOperation : ILoadingOperation
    {
        public string Description => "Main menu loading...";
        public async Task Load(Action<float> onProgress)
        {
            onProgress?.Invoke(0.5f);
            var loadOp = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
            while (!loadOp.isDone)
            {
                await Task.Yield();
            }
            onProgress?.Invoke(1f);
        }
    }
}