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
        public static readonly Dictionary<Player.State, string> SPRITE_PATHS =  new Dictionary<Player.State, string>() {
                                                                                    {Player.State.PUNCHING, "Punch"}, 
                                                                                    {Player.State.KICKING, "Kick"}, 
                                                                                    {Player.State.RAGDOLLING, "Ragdoll"}, 
                                                                                    {Player.State.JUMPING, "Jump"}, 
                                                                                    {Player.State.MISSILE, "Rocket"}, 
                                                                                };
                                                                                
        //player names
        public static readonly string[] PLAYER_NAMES = {"Frog", "Blob", "IceBoi", "LavaBoi"};

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
        public static readonly float KILL_THRESHOLD = 5.0f;

        //tags & layers
        public static readonly string PLAYER_TAG = "Player";
        public static readonly string ENEMY_TAG = "Enemy";
        public static readonly string MAP_LAYER = "Solid";

        //Scene Names
        public static readonly string LOGIN_MENU_SCENE_NAME = "Login_Menu";
        public static readonly string MAIN_MENU_SCENE_NAME = "Main_Menu";
        public static readonly string CONTROLS_SCENE_NAME = "Control_Screen";
        public static readonly string MAP_1_SCENE_NAME = "Map1";

        //Room IDs
        public static readonly string DEFAULT_ROOM_ID = "00000000";

        //
        public static readonly string PLAYER_USERNAME = "You";

        // Webserver Constants
        // Test Server IP : 206.189.214.224
        // Server IP : 206.189.78.132
        public static readonly string MAIN_SERVER_IP = "206.189.78.132:80";
        public static readonly string TEST_SERVER_IP = "206.189.214.224:80";
        public static readonly string SERVER_IP = MAIN_SERVER_IP; // Just change this when switching from one server to another

        // Endpoints
        public static readonly string ADD_USER_ENDPONT = "/adduser";
        public static readonly string USER_STATUS_ENDPONT = "/statususer";
        public static readonly string ROOM_REQUEST_ENDPONT = "/requestroom";
        public static readonly string ROOM_CHECK_ENDPONT = "/checkroom";
        public static readonly string INCREASE_SCORE_ENDPONT = "/incscore";
    }
}
