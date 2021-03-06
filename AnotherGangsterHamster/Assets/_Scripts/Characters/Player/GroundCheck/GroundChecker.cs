using System.Collections;
using System.Collections.Generic;
using Objects;
using UnityEngine;

namespace Characters.Player.GroundCheck
{
   [RequireComponent(typeof(BoxCollider))]
   public class GroundChecker : MonoBehaviour
   {
      public List<string> _tags = new List<string>();
      [SerializeField] private SetObjectParent _setParent = null;

      private IGroundCallback _callback = null;

      private void Awake()
      {
         _callback = GetComponentInChildren<IGroundCallback>();
      }

      private void OnTriggerEnter(Collider other)
      {
         if (_tags.Find(x => other.CompareTag(x)) != null)
         {
            _callback?.OnGround();
            _setParent?.Active(null);
         }
      }

      private void OnTriggerStay(Collider other) // FIXME: 이거 고쳐야 함
      {
         if (_tags.Find(x => other.CompareTag(x)) != null)
         {
            _callback?.StayGround();
            _setParent?.Active(null);
         }
      }

      private void OnTriggerExit(Collider other)
      {
         if (_tags.Find(x => other.CompareTag(x)) != null)
         {
            _callback?.ExitGround();
            _setParent?.Deactive(null);
         }
      }
   }
}