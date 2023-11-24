using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Codebase.Runtime.Networking.Client;
using Codebase.Runtime.Networking.Server.Services;
using Codebase.Runtime.Networking.Shared;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Codebase.Runtime.Networking.Server
{
    public class ServerGameManager : IDisposable
    {
        private string serverIP;
        private int serverPort;
        private int queryPort;
        private MatchplayBackfiller backfiller;
        private MultiplayAllocationService multiplayAllocationService;

        private Dictionary<string, int> teamIdToTeamIndex = new Dictionary<string, int>();

        public NetworkServer NetworkServer { get; private set; }

        public ServerGameManager(string serverIP, int serverPort,
            int queryPort, NetworkManager manager, NetworkObject playerPrefab)
        {
            this.serverIP = serverIP;
            this.serverPort = serverPort;
            this.queryPort = queryPort;
            NetworkServer = new NetworkServer(manager, playerPrefab);
            multiplayAllocationService = new MultiplayAllocationService();
        }

        public async Task StartGameServerAsync()
        {
            await multiplayAllocationService.BeginServerCheck();

            try
            {
                MatchmakingResults matchmakerPayload = await GetMatchmakerPayload();

                if (matchmakerPayload != null)
                {
                    await StartBackfill(matchmakerPayload);
                    NetworkServer.OnUserConnected += UserJoined;
                    NetworkServer.OnUserDisconnected += UserLeft;
                }
                else
                {
                    Debug.LogWarning("Matchmaker payload timed out");
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }

            if (!NetworkServer.OpenConnection(serverIP, serverPort))
            {
                Debug.LogWarning("NetworkServer did not start as expected.");
                return;
            }
        }

        private async Task<MatchmakingResults> GetMatchmakerPayload()
        {
            Task<MatchmakingResults> matchmakerPayloadTask =
                multiplayAllocationService.SubscribeAndAwaitMatchmakerAllocation();

            if (await Task.WhenAny(matchmakerPayloadTask, Task.Delay(20000)) == matchmakerPayloadTask)
            {
                return matchmakerPayloadTask.Result;
            }

            return null;
        }

        private async Task StartBackfill(MatchmakingResults payload)
        {
            backfiller = new MatchplayBackfiller($"{serverIP}:{serverPort}",
                payload.QueueName,
                payload.MatchProperties,
                20);

            if (backfiller.NeedsPlayers())
            {
                await backfiller.BeginBackfilling();
            }
        }

        private void UserJoined(UserData user)
        {
            Team team = backfiller.GetTeamByUserId(user.userAuthId);
            if (!teamIdToTeamIndex.TryGetValue(team.TeamId, out int teamIndex))
            {
                teamIndex = teamIdToTeamIndex.Count;
                teamIdToTeamIndex.Add(team.TeamId, teamIndex);
            }

            user.teamIndex = teamIndex;

            multiplayAllocationService.AddPlayer();
            if (!backfiller.NeedsPlayers() && backfiller.IsBackfilling)
            {
                _ = backfiller.StopBackfill();
            }
        }

        private void UserLeft(UserData user)
        {
            int playerCount = backfiller.RemovePlayerFromMatch(user.userAuthId);
            multiplayAllocationService.RemovePlayer();

            if (playerCount <= 0)
            {
                CloseServer();
                return;
            }

            if (backfiller.NeedsPlayers() && !backfiller.IsBackfilling)
            {
                _ = backfiller.BeginBackfilling();
            }
        }

        private async void CloseServer()
        {
            await backfiller.StopBackfill();
            Dispose();
            Application.Quit();
        }

        public void Dispose()
        {
            NetworkServer.OnUserConnected -= UserJoined;
            NetworkServer.OnUserDisconnected -= UserLeft;

            backfiller?.Dispose();
            multiplayAllocationService?.Dispose();
            NetworkServer?.Dispose();
        }
    }
}