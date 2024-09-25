using Fusion;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    public class StarterAssetsInputs : NetworkBehaviour
    {
        [Header("Character Input Values")]
        public Vector2 move;
        public Vector2 look;
        public bool jump;
        public bool sprint;

        [Header("Movement Settings")]
        public bool analogMovement;

        [Header("Mouse Cursor Settings")]
        public bool cursorLocked = true;
        public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
        public void OnMove(InputValue value)
        {
            if (HasInputAuthority) // Vérifiez l'autorité
            {
                MoveInput(value.Get<Vector2>());
            }
        }

        public void OnLook(InputValue value)
        {
            if (HasInputAuthority && cursorInputForLook) // Vérifiez l'autorité
            {
                LookInput(value.Get<Vector2>());
            }
        }

        public void OnJump(InputValue value)
        {
            if (HasInputAuthority) // Vérifiez l'autorité
            {
                JumpInput(value.isPressed);
            }
        }

        public void OnSprint(InputValue value)
        {
            if (HasInputAuthority) // Vérifiez l'autorité
            {
                SprintInput(value.isPressed);
            }
        }
#endif

        public void MoveInput(Vector2 newMoveDirection)
        {
            if (HasInputAuthority) // Vérifiez l'autorité avant de mettre à jour les valeurs d'entrée
            {
                move = newMoveDirection;
            }
        }


        public void LookInput(Vector2 newLookDirection)
        {
            if (HasInputAuthority) // Vérifiez l'autorité avant de mettre à jour les valeurs d'entrée
            {
                look = newLookDirection;
            }
        }

        public void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
        }

        public void SprintInput(bool newSprintState)
        {
            if (HasInputAuthority) // Vérifiez l'autorité avant de mettre à jour les valeurs d'entrée
            {
                sprint = newSprintState;
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (HasInputAuthority) // Vérifiez l'autorité avant de mettre à jour les valeurs d'entrée
            {
                SetCursorState(cursorLocked);
            }
        }

        private void SetCursorState(bool newState)
        {
            if (HasInputAuthority) // Vérifiez l'autorité avant de mettre à jour les valeurs d'entrée
            {
                Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
            }
        }
    }
}
