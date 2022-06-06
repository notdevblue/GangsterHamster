using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Objects
{
    public class TriggerStayInteractableObject : MonoBehaviour
    {
        public List<CollisionCallback> _callbacks
                 = new List<CollisionCallback>();

        private void OnTriggerStay(Collider other)
        {
            _callbacks.Find(x => (x.key == "") || other.gameObject.CompareTag(x.key))
                ?.OnActive?.
                Invoke(other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            _callbacks.Find(x => (x.key == "") || other.gameObject.CompareTag(x.key))
                ?.OnDeactive?.
                Invoke(other.gameObject);
        }
    }
}