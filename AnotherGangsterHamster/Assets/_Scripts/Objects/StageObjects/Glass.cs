using UnityEngine;

namespace Objects.StageObjects
{
    [RequireComponent(typeof(CollisionInteractableObject))]
    public class Glass : MonoBehaviour, ICollisionEventable
    {
        public float MaximunKineticEnergy = 10.0f;

        CollisionInteractableObject _colInteractable;

        private void Awake()
        {
            _colInteractable = GetComponent<CollisionInteractableObject>();
        }

        public void Active(GameObject other)
        {
            if (other.TryGetComponent<Rigidbody>(out var rigid))
            {
                Debug.Log(_colInteractable.colVelocity.magnitude * rigid.mass);
                if (MaximunKineticEnergy < _colInteractable.colVelocity.magnitude * rigid.mass)
                {
                    gameObject.SetActive(false);
                }
            }
        }

        public void Deactive(GameObject other) { }
    }
}