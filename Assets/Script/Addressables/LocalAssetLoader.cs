using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class LocalAssetLoader
{
    private GameObject _cachedObject;

    protected async Task<T> LoadInternal<T>(string assetId)
    {
        var handle = Addressables.InstantiateAsync(assetId);
        _cachedObject = await handle.Task;

        if (!_cachedObject.TryGetComponent(out T loadingScreen))
            throw new NullReferenceException($"Null from addressables!");

        return loadingScreen;
    }

    protected void UnloadInternal()
    {
        if (_cachedObject == null)
            return;

        _cachedObject.SetActive(false);
        Addressables.ReleaseInstance(_cachedObject);
        _cachedObject = null;
    }
}
