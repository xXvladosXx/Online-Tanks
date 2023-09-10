using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Codebase.Runtime.Input
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "Input/InputReader")]
    public class InputReader : ScriptableObject, Controls.IPlayerActions
    {
        private Controls _controls;
        
        public event Action<Vector2> OnMoved;
        public event Action<bool> OnFired; 

        private void OnEnable()
        {
            if (_controls == null)
            {
                _controls = new Controls();
                _controls.Player.SetCallbacks(this);
            }
            
            _controls.Player.Enable();
        }

        public void OnMove(InputAction.CallbackContext context) => OnMoved?.Invoke(context.ReadValue<Vector2>());

        public void OnFire(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnFired?.Invoke(true);
            }
            else if (context.canceled)
            {
                OnFired?.Invoke(false);
            }
        }
    }
}