﻿using Communicate;
using Communicate.Interfaces;
using Data.Enums;
using Data.Interfaces;
using Data.Models.Player;
using GameServer.Networks.Packets.Response;
using Newtonsoft.Json;
using Utility;

namespace GameServer.Services
{
    public class PlayerService : IPlayerService
    {

        public PlayerService()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        public void SendPlayerLists(ISession session)
        {
            if (session.GetPlayers().Count > 0)
            {
                session.GetPlayers().ForEach(player =>
                {
                    new ResponsePlayerList(player).Send(session);
                });
            }
            else
            new ResponsePlayerList().Send(session);
        }

        /// <summary>
        /// Check exist character name
        /// and then send Response to client
        /// </summary>
        /// <param name="session"></param>
        /// <param name="name"></param>
        public async void CheckNameExist(ISession session, string name)
        {
            bool isExists = await Global
                .ApiService
                .CheckNameExist(name);

            new ResponseCheckName(name, isExists).Send(session);
        }

        /// <summary>
        /// Create Player data and send to ApiServer
        /// Waiting response to send resutl to client
        /// </summary>
        /// <param name="session"></param>
        /// <param name="name"></param>
        /// <param name="playerClass"></param>
        /// <param name="hairColor"></param>
        /// <param name="voice"></param>
        /// <param name="gender"></param>
        public async void CreatePlayer(ISession session, string name, PlayerClass playerClass, string hairColor, int voice, int gender)
        {
            Player player = new Player();
            player.Name = name;
            player.Level = 1;
            player.Job = (int)playerClass;
            player.JobLevel = 1;
            player.AccountId = session.GetAccount().Id;
            player.AccountName = session.GetAccount().Username;
            player.Force = 0;
            player.HairColor = hairColor;
            player.Voice = voice;
            player.Gender = gender;
            player.Title = 0;

            player.Position = new Data.Models.World.Position();
            // todo Start Position in game
            // todo load from data start template
            player.Position.X = 0;
            player.Position.Y = 0;
            player.Position.Z = 0;

            var jsonStr = JsonConvert.SerializeObject(player);
            Log.Debug(jsonStr.PrintJson());

            player = await Global
                .ApiService
                .SendCreatePlayer(player);

            Global
                .FeedbackService
                .OnCreatePlayerResult(session, player);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        public void OnUpdateSetting(ISession session)
        {
            var player = session
                    .GetSelectedPlayer();
            Global
                .VisibleService
                .Send(player, new ResponsePlayerInfo(player));
        }

        public void EnterWorld(Player player)
        {
            // todo
        }

        public void Action()
        {

        }
    }
}
