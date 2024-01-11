using System;
using System.Collections;
using System.Collections.Generic;
using Core.Loading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppStartup : MonoBehaviour
{
    private LoadingScreenProvider loadingProvider => ProjectContext.Instance.LoadingScreenProvider;
    private async void Start()
    {
        ProjectContext.Instance.Initialize();

        var loadingOperations = new Queue<ILoadingOperation>();
        loadingOperations.Enqueue(new MenuLoadingOperation());
        var loadingScreen = await loadingProvider.Load();
        await loadingScreen.Load(loadingOperations);
        loadingProvider.Unload();
        SceneManager.UnloadSceneAsync(0);
    }
}
