﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rebound{
    public class BroadcastPayload
    {
        public Vector2 position;
        public Vector2 velocity;
        public Player.State state;
        public string action;
    }
}
