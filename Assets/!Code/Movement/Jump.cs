using UnityEngine;

public static class Jump {

    public static Vector2 Execute(JumpInput input) {
        return Vector2.up * input.JumpVelocity;
    }
}

public class JumpInput {
    public Rigidbody2D RigidBody;
    public float JumpVelocity;
    public float fallMultiplier;
    public float lowJumpMultiplier;
}