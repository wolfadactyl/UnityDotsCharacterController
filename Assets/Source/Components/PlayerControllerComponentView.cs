using System;
using Unity.Entities;
using UnityEngine;

namespace VertexFragment
{
    /// <summary>
    /// Indicates that the entity is controlled directly by player input.
    /// </summary>
    public struct PlayerControllerComponent : IComponentData
    {
        // Intentionally empty.
    }

    /// <summary>
    /// Used to add <see cref="PlayerControllerComponent"/> via the Editor.
    /// </summary>
    [Serializable]
    public sealed class PlayerControllerComponentView : MonoBehaviour
    {
        public TransformUsageFlags TransformFlags = TransformUsageFlags.Dynamic;
    }

    /// <summary>
    /// Used to bake <see cref="PlayerControllerComponent"/> onto <see cref="PlayerControllerComponentView"/>.
    /// </summary>
    public sealed class PlayerControllerComponentViewBaker : Baker<PlayerControllerComponentView>
    {
        public override void Bake(PlayerControllerComponentView authoring)
        {
            if (!authoring.enabled)
            {
                return;
            }

            Entity entity = this.GetEntity(authoring.TransformFlags);
            AddComponent(entity, new PlayerControllerComponent());
        }
    }
}
