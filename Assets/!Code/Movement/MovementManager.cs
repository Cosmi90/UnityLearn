using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using SuperVania.Input;

namespace SuperVania.Movement {

    public class MovementManager {
        public BoxCollider2D BoxCollider { get; set; }
        public LayerMask PlatformLayer { get; set; }

        public MovementType GetMovementType(List<KeyCode> keysPressed) {
            foreach (KeyCode k in keysPressed) {
                if (k == InputManager.JumpKey) {
                    return MovementType.Jump;
                }
            }

            return MovementType.Stay;
        }

        public void ExecuteMovement(MovementType movementType, Rigidbody2D rb, float moveSpeed, float jumpVelocity) {
            bool isGrounded = IsGrounded();

            if (movementType == MovementType.Jump && isGrounded) {
                rb.velocity = Jump.Execute(new JumpInput { RigidBody = rb, JumpVelocity = jumpVelocity });
            }
            else if (movementType == MovementType.Stay) {
                // pressing no keys
            }
        }

        // TO-DO Change the IsGrounded methos from using BoxCast to Physics2D.OverlapBox
        private bool IsGrounded() {
            RaycastHit2D raycastHit2d = Physics2D.BoxCast(BoxCollider.bounds.center, BoxCollider.bounds.size, 0f, Vector2.down, .1f, PlatformLayer);
            return raycastHit2d.collider != null;
        }
    }

}