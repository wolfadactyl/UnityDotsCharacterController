using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace VertexFragment
{
    /// <summary>
    /// Basic system which follows the entity with the <see cref="CameraFollowComponent"/>.
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup)), UpdateAfter(typeof(ExportPhysicsWorld)), UpdateAfter(typeof(PhysicsSystemGroup))]
    public sealed partial class CameraFollowSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float scroll = Input.GetAxis("Mouse Scroll Wheel");
            float mouse2 = Input.GetAxis("Mouse 2");
            float mouseX = MathUtils.IsZero(mouse2) ? 0.0f : Input.GetAxis("Mouse X");
            float mouseY = MathUtils.IsZero(mouse2) ? 0.0f : Input.GetAxis("Mouse Y");

            Entities.ForEach((
                Entity entity,
                ref LocalTransform localTransform,
                ref CameraFollowComponent camera) =>
            {
                ProcessCameraZoom(ref camera, scroll);
                ProcessCameraYawPitch(ref camera, mouseX, mouseY);

                Vector3 currPos = Camera.main.transform.position;
                Vector3 targetPos = new Vector3(localTransform.Position.x, localTransform.Position.y + 1.0f, localTransform.Position.z);

                targetPos += (Camera.main.transform.forward * -camera.Zoom);
                float posLerp = Mathf.Clamp(SystemAPI.Time.DeltaTime * 8.0f, 0.0f, 1.0f);

                Camera.main.transform.rotation = new Quaternion();
                Camera.main.transform.Rotate(new Vector3(camera.Pitch, camera.Yaw, 0.0f));
                Camera.main.transform.position = Vector3.Lerp(currPos, targetPos, posLerp);

                camera.Forward = Camera.main.transform.forward;
                camera.Right = Camera.main.transform.right;
            }).WithoutBurst().Run();
        }

        /// <summary>
        /// Handles input for zooming the camera in and out.
        /// </summary>
        /// <param name="camera"></param>
        private static void ProcessCameraZoom(ref CameraFollowComponent camera, float scroll)
        {
            if (MathUtils.IsZero(scroll))
            {
                return;
            }

            camera.Zoom -= scroll;
        }

        /// <summary>
        /// Handles input for manipulating the camera yaw (rotating around).
        /// </summary>
        /// <param name="camera"></param>
        private static void ProcessCameraYawPitch(ref CameraFollowComponent camera, float mouseX, float mouseY)
        {
            camera.Yaw += Input.GetAxis("Mouse X");
            camera.Pitch -= Input.GetAxis("Mouse Y");
        }
    }
}
