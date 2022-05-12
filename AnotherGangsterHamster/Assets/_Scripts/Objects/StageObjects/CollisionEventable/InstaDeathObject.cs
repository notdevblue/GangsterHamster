using Characters;
using UnityEngine;

namespace Objects.StageObjects.CollisionEventable
{
   [RequireComponent(typeof(CollisionInteractableObject))]
   public class InstaDeathObject : MonoBehaviour, ICollisionEventable
   {
      public void Active(GameObject other)
      {
         other.GetComponent<CharacterBase>()?.Damage(100000000);
      }

      public void Deactive(GameObject other) { }
   }
}