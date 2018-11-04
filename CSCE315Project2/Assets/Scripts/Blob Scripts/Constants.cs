using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rebound {
    public static class Constants {
        
        //limits on latency
        public static readonly float SPEED_LIMIT = 70f;
        public static readonly float JUMP_SPEED = 40f;
        public static readonly float MOVEMENT_SPEED = 20f;
        public static readonly float PUNCH_SPEED = 30f;
        public static readonly float KICK_SPEED = 30f;
        public static readonly float GRAVITY_SCALE = 5f;

        //paths to sprites (relative to Resources)
        public static readonly string PUNCH_SPRITE_PATH = "Punch";
        public static readonly string KICK_SPRITE_PATH = "Kick";
        public static readonly string RAGDOLL_SPRITE_PATH = "Ragdoll";
        public static readonly string JUMP_SPRITE_PATH = "Jump";

        //player names
        public static readonly string[] PLAYER_NAMES = {"Frog", "Blob", "Ball", "Meanie"};

        //codes of different animation states
        public static readonly int EMPTY_STATE_CODE = 0;
        public static readonly int IDLE_STATE_CODE = 1;

        //masses
        public static readonly float PLAYER_MASS = 5.0f;
        public static readonly float PUNCH_MASS = 1000.0f;
        public static readonly float KICK_MASS = 12.0f;

        //controls
        public static readonly string RIGHT_KEY = "d";
        public static readonly string LEFT_KEY = "a";
        public static readonly string MISSILE_KEY = "w";
        public static readonly string DOWN_KEY = "s";
        public static readonly string PUNCH_KEY = "j";
        public static readonly string KICK_KEY = "k";
        public static readonly string JUMP_KEY = "space";

        //state retention timings in secs
        public static readonly float PUNCH_TIME = 1.0f; 
        public static readonly float KICK_TIME = 1.0f; 
        public static readonly float RAGDOLL_TIME = 3.0f; 

        //corrections
        public static readonly float CORRECTION_THRESHOLD = 2;

        //server-client communication
        public static readonly int UPDATE_FREQUENCY = 5;

        public static readonly List<Vector2> SPAWN_POINTS =
            new List<Vector2>(new[]
        {
            new Vector2(0.0f, 35.0f),
            new Vector2(-35.0f, 13.0f),
            new Vector2(30.0f, 13.0f),
            new Vector2(30.0f, -3.5f),
            new Vector2(-35.0f, -3.5f)
        });

        //miscellaneous
        public static readonly float EPSILON = 0.001f;
        public static readonly int MAX_PLAYERS = 4;
    }
}
