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
            if (HasInputAuthority) // V�rifiez l'autorit�
            {
                MoveInput(value.Get<Vector2>());
            }
        }

        public void OnLook(InputValue value)
        {
            if (HasInputAuthority && cursorInputForLook) // V�rifiez l'autorit�
            {
                LookInput(value.Get<Vector2>());
            }
        }

        public void OnJump(InputValue value)
        {
            if (HasInputAuthority) // V�rifiez l'autorit�
            {
                JumpInput(value.isPressed);
            }
        }

        public void OnSprint(InputValue value)
        {
            if (HasInputAuthority) // V�rifiez l'autorit�
            {
                SprintInput(value.isPressed);
            }
        }
#endif

        public void MoveInput(Vector2 newMoveDirection)
        {
            if (HasInputAuthority) // V�rifiez l'autorit� avant de mettre � jour les valeurs d'entr�e
            {
                move = newMoveDirection;
            }
        }


        public void LookInput(Vector2 newLookDirection)
        {
            if (HasInputAuthority) // V�rifiez l'autorit� avant de mettre � jour les valeurs d'entr�e
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
            if (HasInputAuthority) // V�rifiez l'autorit� avant de mettre � jour les valeurs d'entr�e
            {
                sprint = newSprintState;
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (HasInputAuthority) // V�rifiez l'autorit� avant de mettre � jour les valeurs d'entr�e
            {
                SetCursorState(cursorLocked);
            }
        }

        private void SetCursorState(bool newState)
        {
            if (HasInputAuthority) // V�rifiez l'autorit� avant de mettre � jour les valeurs d'entr�e
            {
                Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
            }
        }
    }
}
