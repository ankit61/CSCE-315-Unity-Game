using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rebound {
    public static class Constants {
        
        //limits on latency
        public const float SPEED_LIMIT = 70f;
        public const float JUMP_SPEED = 40f;
        public const float MOVEMENT_SPEED = 20f;
        public const float PUNCH_SPEED = 20f;
        public const float KICK_SPEED = 30f;
        public const float GRAVITY_SCALE = 4f;

        //paths to sprites (relative to Resources)
        public const string PUNCH_SPRITE_PATH = "Punch";
        public const string KICK_SPRITE_PATH = "Kick";

        //codes of different animation states
        public const int EMPTY_STATE_CODE = 0;
        public const int IDLE_STATE_CODE = 1;

        //masses
        public const float PLAYER_MASS = 5.0f;
        public const float PUNCH_MASS = 1000.0f;
        public const float KICK_MASS = 12.0f;

        //controls
        public const string RIGHT_KEY = "d";
        public const string LEFT_KEY = "a";
        public const string MISSILE_KEY = "w";
        public const string DOWN_KEY = "s";
        public const string PUNCH_KEY = "j";
        public const string KICK_KEY = "k";
        public const string JUMP_KEY = "space";

        //state retention timings in secs
        public const float PUNCH_TIME = 1.0f; 
        public const float KICK_TIME = 1.0f; 
        public const float RAGDOLL_TIME = 3.0f; 

        //corrections
        public const float CORRECTION_THRESHOLD = 2;

        //server-client communication
        public const int UPDATE_FREQUENCY = 5;

        public static readonly List<Vector2> SPAWN_POINTS =
            new List<Vector2>(new[]
        {
            new Vector2(0.0f, 35.0f),
            new Vector2(-35.0f, 13.0f),
            new Vector2(30.0f, 13.0f),
            new Vector2(30.0f, -3.5f),
            new Vector2(-35.0f, -3.5f)
        });

    }
}
