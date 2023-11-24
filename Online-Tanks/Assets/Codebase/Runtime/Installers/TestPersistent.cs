using System;
using UnityEngine;
using Zenject;

namespace Codebase.Runtime.Installers
{
    public class TestPersistent : IInitializable, IDisposable
    {
        public void Initialize()
        {
            
        }

        public void Dispose()
        {
            Debug.Log("TestPersistent.Dispose()");
        }
    }
}