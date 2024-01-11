using System;
using UnityEngine;

namespace Core.Loading
{
    public class ProjectContext : MonoBehaviour
    {
        public static ProjectContext Instance;
    
        public LoadingScreenProvider LoadingScreenProvider { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            DontDestroyOnLoad(this);
        }

        public void Initialize()
        {
            LoadingScreenProvider = new LoadingScreenProvider();
        }
    }
}