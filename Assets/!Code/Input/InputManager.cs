using UnityEngine;
using UnityInput = UnityEngine.Input; // In order to avoid overlap with SuperVania.Input

using System.Collections.Generic;
using System.Linq;

namespace SuperVania.Input {

    public class InputManager {
        // Keys are statically defined for now ***
        public static KeyCode UpKey        = KeyCode.UpArrow;
        public static KeyCode DownKey      = KeyCode.DownArrow;
        public static KeyCode LeftKey      = KeyCode.LeftArrow;
        public static KeyCode RightKey     = KeyCode.RightArrow;
        public static KeyCode AttackKey    = KeyCode.Keypad4;
        public static KeyCode JumpKey      = KeyCode.Keypad5;
        public static KeyCode CancelKey    = KeyCode.Keypad8;
        public static KeyCode DashKey      = KeyCode.Keypad7;
        // ****************************************

        private List<KeyCode> allKeys = new List<KeyCode>();

        public InputManager() {
            InitializeKeys();
            CheckKeysDuplicates();
        }

        public List<KeyCode> GetKeysPressed() {
            List<KeyCode> pressedKeys = new List<KeyCode>();

            foreach (KeyCode k in allKeys) {
                if (UnityInput.GetKey(k)) {
                    pressedKeys.Add(k);
                }
                else if (!UnityInput.anyKey) {
                    pressedKeys.Add(KeyCode.None);
                }
            }

            foreach (KeyCode k in pressedKeys) {
                Debug.Log($"Keypressed: {k}");
            }

            return pressedKeys;
        }

        private void InitializeKeys() {
            allKeys.Add(UpKey);
            allKeys.Add(DownKey);
            allKeys.Add(LeftKey);
            allKeys.Add(RightKey);
            allKeys.Add(AttackKey);
            allKeys.Add(JumpKey);
            allKeys.Add(CancelKey);
            allKeys.Add(DashKey);
        }

        private void CheckKeysDuplicates() {
            if (allKeys.Count != allKeys.Distinct().Count()) {
                Debug.LogError("Duplicate keys detected !");
            }
        }
    }

}