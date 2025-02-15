﻿using Data.Models.World;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Data.Models.Creature
{
    public class Creature : VisibleObject
    {
        [JsonIgnore]
        public List<Player.Player> VisiblePlayers = new List<Player.Player>();

        private MapInstance Map;

        public MapInstance GetMap()
        {
            return Map;
        }

        public void SetMap(MapInstance map)
        {
            Map = map;
        }
    }
}
