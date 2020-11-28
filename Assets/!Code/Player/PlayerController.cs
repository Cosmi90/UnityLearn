using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using SuperVania.Input;
using SuperVania.Movement;

namespace SuperVania.Player {

    public class PlayerController : MonoBehaviour {
        public float moveSpeed;
        public float jumpVelocity;
        public LayerMask platformLayer;

        private InputManager IM;
        private MovementManager MM;
        private MovementType MT;
        private Rigidbody2D rigidbody2d;

        private void Awake() {
            IM = new InputManager();
            MM = new MovementManager{ BoxCollider = transform.GetComponent<BoxCollider2D>(), PlatformLayer = platformLayer };
            rigidbody2d = transform.GetComponent<Rigidbody2D>();
        }

        // Start is called before the first frame update
        void Start() {

        }

        // Update is called once per frame
        void Update() {
            MT = MM.GetMovementType(IM.GetKeysPressed());
        }

        private void FixedUpdate() {
            MM.ExecuteMovement(MT, rigidbody2d, moveSpeed, jumpVelocity);
        }
    }
}