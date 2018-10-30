using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rebound {
    public static class Constants {
        public const float SPEED_LIMIT = 70f;
        public const float JUMP_SPEED = 20f;
        public const float MOVEMENT_SPEED = 10f;
        public const float PUNCH_SPEED = 20f;
        public const float KICK_SPEED = 30f;

        public const string PUNCH_SPRITE_PATH = "Punch";
        public const string KICK_SPRITE_PATH = "Kick";

        public const int EMPTY_STATE_CODE = 0;
        public const int IDLE_STATE_CODE = 1;

        public const float PLAYER_MASS = 5.0f;
        public const float PUNCH_MASS = 1000.0f;
        public const float KICK_MASS = 12.0f;

    }
}
