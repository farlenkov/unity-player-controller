using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityGameLoop;

namespace UnityPlayerController
{
    public partial class PlayerMoveSystem : GameLoopSystem<GameLoop>
    {
        PlayerControllerConfig config;
        InputAction moveAction;
        InputAction jumpAction;
        InputAction crouchAction;
        InputAction sprintAction;

        protected override GameLoopFuncList UpdateList => Loop.FixedUpdate;

        protected override void OnInit()
        {
            base.OnInit();

            config = PlayerControllerConfig.Load();
            moveAction = config.MoveInputAction?.action;
            jumpAction = config.JumpInputAction?.action;
            crouchAction = config.CrouchInputAction?.action;
            sprintAction = config.SprintInputAction?.action;

            moveAction.Enable();
        }

        protected override void OnUpdate()
        {
            Entities
                .WithoutBurst()
                .ForEach((PlayerBody playerBody) =>
                {
                    OnUpdate(playerBody);

                }).Run();
        }
        
        void OnUpdate(PlayerBody playerBody)
        {
            Entities
                .WithoutBurst()
                .ForEach((PlayerVerticalRotation verticalRotation) =>
                {
                    OnUpdate(playerBody, verticalRotation);

                }).Run();
        }

        void OnUpdate(
            PlayerBody playerBody,
            PlayerVerticalRotation verticalRotation)
        {
            var forward = (new Vector3(verticalRotation.transform.forward.x, 0, verticalRotation.transform.forward.z)).normalized;
            var right = (new Vector3(verticalRotation.transform.right.x, 0, verticalRotation.transform.right.z)).normalized;

            var moveInput = moveAction.ReadValue<Vector2>().normalized;
            var desiredMove = forward * moveInput.y + right * moveInput.x;

            var oldVelocity = playerBody.Rigidbody.velocity;
            var newVelocity = desiredMove * config.BaseSpeed;

            playerBody.Rigidbody.velocity = Vector3.Lerp(oldVelocity, newVelocity, config.LerpSpeed);
        }
    }
}
