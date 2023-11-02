// in order to circumvent API breakages that do not affect physics, some packages are removed from the project on CI
// any code referencing APIs in com.unity.inputsystem must be guarded behind UNITY_INPUT_SYSTEM_EXISTS
using CharacterController;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace VertexFragment
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    partial class InputGatheringSystem : SystemBase
    {
        EntityQuery m_CharacterControllerInputQuery;
        EntityQuery m_CharacterGunInputQuery;
        EntityQuery m_VehicleInputQuery;

#pragma warning disable 649
        Vector2 m_CharacterMovement;
        Vector2 m_CharacterLooking;
        bool m_CharacterJumped;
#pragma warning restore 649

        protected override void OnCreate()
        {
#if UNITY_INPUT_SYSTEM_EXISTS
            m_InputActions = new InputActions();
            m_InputActions.CharacterController.SetCallbacks(this);
            m_InputActions.Vehicle.SetCallbacks(this);
#endif

            m_CharacterControllerInputQuery = GetEntityQuery(typeof(CharacterController.Input));
        }

#if UNITY_INPUT_SYSTEM_EXISTS
        InputActions m_InputActions;

        protected override void OnStartRunning() => m_InputActions.Enable();

        protected override void OnStopRunning() => m_InputActions.Disable();

        void InputActions.ICharacterControllerActions.OnMove(InputAction.CallbackContext context) => m_CharacterMovement = context.ReadValue<Vector2>();
        void InputActions.ICharacterControllerActions.OnLook(InputAction.CallbackContext context) => m_CharacterLooking = context.ReadValue<Vector2>();
        void InputActions.ICharacterControllerActions.OnFire(InputAction.CallbackContext context) => m_CharacterFiring = context.ReadValue<float>();
        void InputActions.ICharacterControllerActions.OnJump(InputAction.CallbackContext context) { if (context.started) m_CharacterJumped = true; }
#endif

        protected override void OnUpdate()
        {
            // character controller
            if (m_CharacterControllerInputQuery.CalculateEntityCount() == 0)
                EntityManager.CreateEntity(typeof(CharacterController.Input));

            m_CharacterControllerInputQuery.SetSingleton(new CharacterController.Input
            {
                Looking = m_CharacterLooking,
                Movement = m_CharacterMovement,
                Jumped = m_CharacterJumped ? 1 : 0
            });

            m_CharacterJumped = false;
        }
    }
}