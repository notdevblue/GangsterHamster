using Objects.Interaction;
using Matters.Gravity;
using UnityEngine;
using System;

namespace Characters.Player.Actions
{
   [RequireComponent(typeof(Rigidbody))]
   public class Actions : MonoBehaviour, IActionable
   {
      private Rigidbody _rigid;

      private Transform _playerTopTrm = null;
      private Transform PlayerTopTrm
      {
         get
         {
            if (_playerTopTrm == null)
               _playerTopTrm = GameObject.FindWithTag("PLAYER_TOP").transform;

            return _playerTopTrm;
         }
      }

      private Transform _playerTrm = null;
      private Transform PlayerTrm
      {
         get
         {
            if (_playerTrm == null)
               _playerTrm = GameObject.FindWithTag("PLAYER").transform;

            return _playerTrm;
         }
      }

      private CapsuleCollider _collider = null;
      private CapsuleCollider Collider
      {
         get
         {
            if (_collider == null)
            {
               _collider = GameObject.FindWithTag("PLAYER_BASE")
                                     .GetComponent<CapsuleCollider>();
            }

            return _collider;
         }
      }

      private Vector3 _jumpForce;
      private bool _pendingCrouchStand = false; // 일어서야 하는지
      private int _ignoreLayer;

      private void Awake() {
         float force = Mathf.Sqrt(2.0f *
                                      9.8f * // Gravity
                                      PlayerValues.JumpHeight);

         _jumpForce = new Vector3(force, force, force);
         _rigid = GetComponent<Rigidbody>();
         _ignoreLayer = 1 << LayerMask.GetMask("PLAYER");
      }

      public void CrouchStart()
      {
         Vector3 targetScale  = PlayerTrm.localScale;
         Vector3 targetPos    = PlayerTrm.localPosition;

         PlayerStatus.IsCrouching   = true;
         PlayerValues.Speed         = PlayerValues.CrouchSpeed;

         targetScale.y  = PlayerValues.PlayerCrouchYScale;
         targetPos.y    = PlayerValues.PlayerCrouchYPos;

         Collider.center
            = new Vector3(0.0f, PlayerValues.PlayerCrouchHeight / 2.0f, 0.0f);
         Collider.height = Collider.center.y * 2.0f;

         PlayerTrm.localScale    = targetScale;
         PlayerTrm.localPosition = targetPos;
      }

      public void CrouchEnd()
      {
         // 플레이어 천장 체크
         if (UnityEngine.Physics.Raycast(PlayerTopTrm.position,
                                         PlayerTrm.up,
                                         0.9f,
                                         _ignoreLayer
                                         ))
         {
            _pendingCrouchStand = true;
            return;
         }
         _pendingCrouchStand = false;

         Vector3 targetScale  = PlayerTrm.localScale;
         Vector3 targetPos    = PlayerTrm.localPosition;

         PlayerStatus.IsCrouching   = false;
         PlayerValues.Speed         = PlayerValues.DashSpeed;

         targetScale.y  = PlayerValues.PlayerStandingYScale;
         targetPos.y    = PlayerValues.PlayerStandingYPos;

         Collider.center
            = new Vector3(0.0f, PlayerValues.PlayerStandingHeight / 2.0f, 0.0f);
         Collider.height = Collider.center.y * 2.0f;

         
         PlayerTrm.localScale    = targetScale;
         PlayerTrm.localPosition = targetPos;
      }

      public void Jump()
      {
         if(!PlayerStatus.Jumpable) return;

         // 웅크리고 있는 경우 새우기만 함
         if(PlayerStatus.IsCrouching)
         {
            CrouchEnd();
            return;
         }

         Vector3 force      = _jumpForce;
         Vector3 gravityDir = GravityManager.GetGlobalGravityDirection();

         // 중력 방향에 맞게
         force.x *= gravityDir.x;
         force.y *= gravityDir.y;
         force.z *= gravityDir.z;

         _rigid.velocity = -force;

         PlayerStatus.IsJumping = true;
      }

      public void Interact(Action<Transform> onPickup = null)
      {
         InteractionManager.Instance.Interact(onPickup);
      }

      private void FixedUpdate()
      {
         // 플레이어 못 일어난 경우
         if (_pendingCrouchStand)
            CrouchEnd();
      }
   }
}