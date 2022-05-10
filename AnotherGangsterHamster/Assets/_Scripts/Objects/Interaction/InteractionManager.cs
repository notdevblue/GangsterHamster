using UnityEngine;

namespace Objects.Interaction
{  
   public class InteractionManager : Singleton<InteractionManager>
   {
      Interactable   _currentActiveInteraction;
      Transform      _currentActiveAtype;

      private bool _grep = false; // 잡기 상태

      public void SetActiveAtype(Transform transform)
      {
         if (!_grep) // 잡기 상태가 아닐 시
         {
            _currentActiveAtype = transform;
         }
      }

      public void UnGrep()
      {
         _grep = false;
         ClearActvieAtype();
      }

      public void Grep()
      {
         _grep = true;
      }

      public bool GetGrep() => _grep;

      public void ClearActvieAtype()
      {
         if (!_grep)
         {
            _currentActiveAtype = null;
            _grep = false;
         }
      }

      public void SetInteraction(Interactable interactable)
               => _currentActiveInteraction = interactable;

      public void UnSetInteraction(Interactable interactable)
      {
         if (_currentActiveInteraction == interactable)
            _currentActiveInteraction = null;
      }

      /// <summary>
      /// 상호작용 가능한 오브젝트를 null 로 바꿉니다.
      /// </summary>
      public void ClearInteraction()
            => _currentActiveInteraction = null;

      /// <summary>
      /// 상호작용 합니다.<br/>
      /// 들기 가능한 오브젝트가 있다면 듭니다.
      /// </summary>
      public void Interact(System.Action<Transform> onPickup = null)
      {
         if (_currentActiveInteraction != null)
         {
            _currentActiveInteraction.Interact();
         }
         if (_currentActiveAtype != null)
         {
            onPickup?.Invoke(_currentActiveAtype);
         }
      }

      // 의존성 때문에 이렇게 함
      // 상호작용 메니저에서 값 얻어와서 
      // 부피 계산하고 플레이어 손에 들려주기 싫었음

   }
}