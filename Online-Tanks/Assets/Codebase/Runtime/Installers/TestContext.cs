using System;
using UnityEngine;
using Zenject;

namespace Codebase.Runtime.Installers
{
    public class TestContext : IInitializable, IDisposable
    {
        public void Initialize()
        {
            
        }

        public void Dispose()
        {
            Debug.Log("TestContext.Dispose()");
        }
    }
}