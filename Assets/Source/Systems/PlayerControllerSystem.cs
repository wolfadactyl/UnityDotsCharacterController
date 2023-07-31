using Unity.Entities;
using UnityEngine;

namespace VertexFragment
{
    /// <summary>
    /// Main control system for player input.
    /// </summary>
    public partial class PlayerControllerSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            InputAxes inputAxes = new InputAxes()
            {
                Right = Input.GetAxis("Move Right"),
                Left = Input.GetAxis("Move Left"),
                Forward = Input.GetAxis("Move Forward"),
                Backward = Input.GetAxis("Move Backward"),
                Jump = Input.GetAxis("Jump"),
                Sprint = Input.GetKey(KeyCode.LeftShift),
            };

            Entities.WithAll<PlayerControllerComponent>().ForEach((
                Entity entity,
                ref CameraFollowComponent camera,
                ref CharacterControllerComponent controller) =>
            {
                ProcessMovement(ref inputAxes, ref controller, ref camera);
            }).Run();
        }

        /// <summary>
        /// Processes the horizontal movement input from the player to move the entity along the xz plane.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="camera"></param>
        private static void ProcessMovement(ref InputAxes inputAxes, ref CharacterControllerComponent controller, ref CameraFollowComponent camera)
        {
            float movementX = (inputAxes.Right > 0.0f ? 1.0f : 0.0f) + (inputAxes.Left > 0.0f ? -1.0f : 0.0f);
            float movementZ = (inputAxes.Forward > 0.0f ? 1.0f : 0.0f) + (inputAxes.Backward > 0.0f ? -1.0f : 0.0f);

            Vector3 forward = new Vector3(camera.Forward.x, 0.0f, camera.Forward.z).normalized;
            Vector3 right = new Vector3(camera.Right.x, 0.0f, camera.Right.z).normalized;

            if (!MathUtils.IsZero(movementX) || !MathUtils.IsZero(movementZ))
            {
                controller.CurrentDirection = ((forward * movementZ) + (right * movementX)).normalized;
                controller.CurrentMagnitude = inputAxes.Sprint ? 1.5f : 1.0f;
            }
            else
            {
                controller.CurrentMagnitude = 0.0f;
            }

            controller.Jump = inputAxes.Jump > 0.0f;
        }

        private struct InputAxes
        {
            public float Right { get; set; }
            public float Left { get; set; }
            public float Forward { get; set; }
            public float Backward { get; set; }
            public float Jump { get; set; }
            public bool Sprint { get; set; }
        }
    }
}
