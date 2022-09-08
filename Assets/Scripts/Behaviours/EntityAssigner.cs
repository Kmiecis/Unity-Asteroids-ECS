using Unity.Entities;
using UnityEngine;

namespace Asteroids
{
    public class EntityAssigner : MonoBehaviour, IConvertGameObjectToEntity
    {
        public EntityAssignee[] targets;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            if (targets != null)
            {
                foreach (var target in targets)
                {
                    target.entity = entity;
                }
            }
        }
    }
}
