using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rebound {
    public static class Constants {
        
        //limits on latency
        public static readonly float SPEED_LIMIT = 90f;
        public static readonly float JUMP_SPEED = 45f;
        public static readonly float MOVEMENT_SPEED = 30f;
        public static readonly float PUNCH_SPEED = 40f;
        public static readonly float KICK_SPEED = 40f;
        public static readonly float DOWN_SPEED = 40f;
        public static readonly float MISSILE_SPEED = 70f;
        public static readonly float GRAVITY_SCALE = 5f;

        //cooldowns
        public static readonly Dictionary<Player.State, float> COOLDOWNS = new Dictionary<Player.State, float>() {
                                                                                {Player.State.MISSILE, 2f},
                                                                                {Player.State.KICKING, 3f},
                                                                            };

        //available actions
        public static readonly Dictionary<Player.State, int> NUM_AVAILABLE_ACTIONS = new Dictionary<Player.State, int>() {
                                                                                {Player.State.MISSILE, 1},
                                                                                {Player.State.KICKING, 2},
                                                                            };

        //paths to sprites (relative to Resources)
        public static readonly string PUNCH_SPRITE_PATH = "Punch";
        public static readonly string KICK_SPRITE_PATH = "Kick";
        public static readonly string RAGDOLL_SPRITE_PATH = "Ragdoll";
        public static readonly string JUMP_SPRITE_PATH = "Jump";
        public static readonly string MISSILE_SPRITE_PATH = "Rocket";
        public static readonly string ROCK_SPRITE_PATH = "Rock";

        //player names
        public static readonly string[] PLAYER_NAMES = {"Frog", "Blob"}; // , "Ball", "Meanie"

        //codes of different animation states
        public static readonly int EMPTY_STATE_CODE = 0;
        public static readonly int IDLE_STATE_CODE = 1;

        //controls
        public static readonly string RIGHT_KEY1 = "d";
        public static readonly string RIGHT_KEY2 = "right";
        public static readonly string LEFT_KEY1 = "a";
        public static readonly string LEFT_KEY2 = "left";
        public static readonly string DOWN_KEY1 = "s";
        public static readonly string DOWN_KEY2 = "down";
        public static readonly string MISSILE_KEY1 = "w";
        public static readonly string MISSILE_KEY2 = "up";
        public static readonly string PUNCH_KEY = "j";
        public static readonly string KICK_KEY = "k";
        public static readonly string JUMP_KEY = "space";

        //state retention timings in secs
        public static readonly Dictionary<Player.State, float> STATE_TIMES = new Dictionary<Player.State, float>() {
                                                            {Player.State.PUNCHING, 1.0f},
                                                            {Player.State.KICKING, 1.0f},
                                                            {Player.State.RAGDOLLING, 3.0f},
                                                            {Player.State.MISSILE, 1.0f },
                                                            {Player.State.ROCK, 1.0f },
                                                        };


        //server-client communication
        public static readonly int UPDATE_FREQUENCY = 3;

        public static readonly List<Vector2> SPAWN_POINTS =
            new List<Vector2>(new[]
        {
            new Vector2(0.0f, 35.0f),
            new Vector2(-35.0f, 13.0f),
            new Vector2(30.0f, 13.0f),
            new Vector2(30.0f, -3.5f),
            new Vector2(-35.0f, -3.5f)
        });

        public static readonly float CORRECTION_THRESHOLD = 4;
        
        //miscellaneous
        public static readonly float EPSILON = 0.001f;
        public static readonly int MAX_PLAYERS = 4;

        //tags & layers
        public static readonly string PLAYER_TAG = "Player";
        public static readonly string ENEMY_TAG = "Enemy";
        public static readonly string MAP_LAYER = "Solid";
    }
}
