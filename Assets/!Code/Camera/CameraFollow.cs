using UnityEngine;

namespace SuperVania.Camera {

    public class CameraFollow : MonoBehaviour {
        public Transform player;

        private void FixedUpdate() {
            transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
        }
    }

}