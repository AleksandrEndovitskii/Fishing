using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Extensions;
using Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Unity.Netcode;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Managers
{
    public class LobbyManager : BaseManager<LobbyManager>
    {
        public event Action<Lobby> OnJoinedLobby = delegate {};

        public Player LoggedInPlayer { get; private set; }
        public Lobby CurrentLobby { get; private set; }
        public List<Lobby> LobbyList { get; private set; }
        public List<Lobby> JoinedLobbies { get; private set; }
        public List<string> JoinedLobbyIds { get; private set; }

        public string CurrentLobbyRelayJoinCode
        {
            get
            {
                var result = CurrentLobby?.Data[KEY_START_GAME].Value;
                return result;
            }
        }
        public bool IsInLobby
        {
            get
            {
                var result = CurrentLobbyRelayJoinCode != 0.ToString();
                return result;
            }
        }

        private bool IsLobbyHost
        {
            get
            {
                var result = IsInLobby &&
                             CurrentLobby?.HostId == LoggedInPlayer?.Id;
                return result;
            }
        }
        private bool IsPlayerInLobby => true;
        private bool IsPlayerWasKickedOutFromLobby => !IsPlayerInLobby;

        // TODO: temp solution - add correct implementation in future
        private bool IsEnoughPlayersConnected
        {
            get
            {
                var result = NetworkManager.Singleton.ConnectedClients.Count >= MINIMAL_PLAYERS_COUNT_REQUIRED_FOR_GAME_START;

                return result;
            }
        }

        private const string KEY_START_GAME = "KEY_START_GAME";
        private const string KEY_GAME_MODE = "StartGame_RelayCode";
        private const string KEY_PLAYER_CHARACTER = "KEY_PLAYER_CHARACTER";
        private const string KEY_PLAYER_NAME = "KEY_PLAYER_NAME";
        private const float LOBBY_POOL_TIMER_MAX = 2f;

        private const int MINIMAL_PLAYERS_COUNT_REQUIRED_FOR_GAME_START = 1;

        // https://docs.unity.com/ugs/en-us/manual/lobby/manual/rate-limits
        private const float LOBBIES_REQUEST_COOLDOWN_SECONDS_COUNT = 2;
        private const float LOBBIES_REQUEST_COOLDOWN_MILLISECONDS_COUNT = LOBBIES_REQUEST_COOLDOWN_SECONDS_COUNT * 1000;

        private float _lobbyPoolTimer;

        protected override async UniTask Initialize()
        {
            Debug.Log($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Started");

            await UniTask.WaitUntil(() => AuthenticationManager.Instance != null &&
                                          AuthenticationManager.Instance.IsInitialized &&
                                          AuthenticationManager.Instance.IsAuthorized);

            LoggedInPlayer = new Player(AuthenticationManager.Instance.PlayerId, null, new Dictionary<string, PlayerDataObject>());

            await UpdateLobbyList();
            JoinedLobbies = LobbyList.Where(l =>
                l.Players.Any(p =>
                    p.Id == LoggedInPlayer.Id)).ToList();
            var joinedLobby = JoinedLobbies.FirstOrDefault();
            var isJoinedToAtLeastOneLobby = joinedLobby != null;
            if (isJoinedToAtLeastOneLobby)
            {
                Debug.Log($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                                    $"\n{nameof(isJoinedToAtLeastOneLobby)} == {isJoinedToAtLeastOneLobby}" +
                                    $"\n{nameof(joinedLobby)} == {joinedLobby}");

                CurrentLobby = await LobbyService.Instance.ReconnectToLobbyAsync(joinedLobby.Id);

                await StartGame();
                IsInitialized = true;

                Debug.Log($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Completed");

                return;
            }

            var existingLobby = LobbyList.FirstOrDefault();
            var isAtLeastOneLobbyExists = existingLobby != null;
            if (isAtLeastOneLobbyExists)
            {
                Debug.Log($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                                    $"\n{nameof(isAtLeastOneLobbyExists)} == {isAtLeastOneLobbyExists}");

                CurrentLobby = await JoinLobbyAsync(existingLobby);

                await StartGame();
                IsInitialized = true;

                Debug.Log($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Completed");

                return;
            }

            CurrentLobby = await CreateLobbyAndJoinWithHeartbeatAsync(LoggedInPlayer.Id + "_lobby", 4, false, GameMode.DeathMatch);

            await StartGame();
            IsInitialized = true;

            this.StartCoroutine(UpdateLobbyListCoroutine());

            Debug.Log($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Completed");
        }

        protected override async UniTask UnInitialize()
        {
            Debug.Log($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Started");

            await LeaveJoinedLobbiesAsync();

            IsInitialized = false;

            Debug.Log($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Completed");
        }

        protected override async UniTask Subscribe()
        {
        }

        protected override async UniTask UnSubscribe()
        {
        }

        private async UniTask<List<Lobby>> GetLobbiesAsync()
        {
            var lobbies = new List<Lobby>();
            try
            {
                var queryLobbiesOptions = new QueryLobbiesOptions
                {
                    Count = 25,
                    // only lobbies with count of available slots greater than 0 - have free slots
                    Filters = new List<QueryFilter>()
                    {
                        new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT),
                    },
                    // lobbies ordered ascended by date of creation - newest first
                    Order = new List<QueryOrder>()
                    {
                        new QueryOrder(false, QueryOrder.FieldOptions.Created),
                    }
                };
                // await UniTask.Delay(LOBBIES_REQUEST_COOLDOWN_MILLISECONDS_COUNT);
                var queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);
                lobbies = queryResponse.Results;
            }
            catch (LobbyServiceException lobbyServiceException)
            {
                Debug.LogError($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Failed" +
                                         $"\n{nameof(lobbyServiceException)}.{nameof(lobbyServiceException.Reason)} == {lobbyServiceException.Reason}");

                throw;
            }

            return lobbies;
        }
        private async UniTask<List<string>> GetJoinedLobbyIdsAsync()
        {
            Debug.Log($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Started");

            var lobbyIds = new List<string>();
            try
            {
                //var lobbies = new List<Lobby>();

                // await UniTask.Delay(LOBBIES_REQUEST_COOLDOWN_MILLISECONDS_COUNT);
                lobbyIds = await LobbyService.Instance.GetJoinedLobbiesAsync();
                // foreach (string lobbyId in lobbyIds)
                // {
                //     var lobby = await LobbyService.Instance.GetLobbyAsync(lobbyId);
                //     lobbies.Add(lobby);
                // }
            }
            catch (LobbyServiceException lobbyServiceException)
            {
                Debug.LogError($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                                         $"\n{nameof(lobbyServiceException)}.{nameof(lobbyServiceException.Reason)} == {lobbyServiceException.Reason}");

                return null;
            }

            Debug.Log($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Completed" +
                                $"\n{nameof(lobbyIds)} == {lobbyIds}");

            return lobbyIds;
        }

        public async UniTask<Lobby> CreateLobbyAndJoinAsync(string lobbyName, int maxPlayers, bool isPrivate, GameMode gameMode)
        {
            Debug.Log($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Started" +
                                $"\n{nameof(lobbyName)} == {lobbyName}" +
                                $"\n{nameof(maxPlayers)} == {maxPlayers}" +
                                $"\n{nameof(isPrivate)} == {isPrivate}" +
                                $"\n{nameof(gameMode)} == {gameMode}");

            var createLobbyOptions = new CreateLobbyOptions()
            {
                Player = LoggedInPlayer,
                IsPrivate = isPrivate,
                Data = new Dictionary<string, DataObject>()
                {
                    {
                        KEY_GAME_MODE, new DataObject(DataObject.VisibilityOptions.Public, gameMode.ToString())
                    },
                    {
                        KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Member, "0")
                    },
                }
            };

            // await UniTask.Delay(LOBBIES_REQUEST_COOLDOWN_MILLISECONDS_COUNT);
            var lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);

            Debug.Log($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Completed" +
                                $"\n{nameof(lobby)} == {JsonConvert.SerializeObject(lobby)}");

            return lobby;
        }
        private async UniTask<Lobby> CreateLobbyAndJoinWithHeartbeatAsync(string lobbyName, int maxPlayers, bool isPrivate, GameMode gameMode)
        {
            Debug.Log($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Started" +
                                $"\n{nameof(lobbyName)} == {lobbyName}" +
                                $"\n{nameof(maxPlayers)} == {maxPlayers}" +
                                $"\n{nameof(isPrivate)} == {isPrivate}" +
                                $"\n{nameof(gameMode)} == {gameMode}");

            var lobby = await CreateLobbyAndJoinAsync(lobbyName, maxPlayers, isPrivate, gameMode);

            this.InvokeActionPeriodically(() =>
            {
                LobbyService.Instance.SendHeartbeatPingAsync(lobby.Id);
            }, 15f);

            Debug.Log($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Completed" +
                                $"\n{nameof(lobby)} == {JsonConvert.SerializeObject(lobby)}");

            return lobby;
        }

        public async UniTask JoinLobbyAsync(string lobbyCode)
        {
            Debug.Log($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Started" +
                                $"\n{nameof(lobbyCode)} == {lobbyCode}");

            var joinLobbyByCodeOptions = new JoinLobbyByCodeOptions()
            {
                Player = LoggedInPlayer,
            };
            // await UniTask.Delay(LOBBIES_REQUEST_COOLDOWN_MILLISECONDS_COUNT);
            var lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);

            CurrentLobby = lobby;

            OnJoinedLobby.Invoke(CurrentLobby);

            Debug.Log($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Completed" +
                                $"\n{nameof(lobby)} == {JsonConvert.SerializeObject(lobby)}");
        }
        public async UniTask<Lobby> JoinLobbyAsync(Lobby lobby)
        {
            Debug.Log($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Started" +
                                $"\n{nameof(lobby)} == {JsonConvert.SerializeObject(lobby)}");

            var joinLobbyByIdOptions = new JoinLobbyByIdOptions()
            {
                Player = LoggedInPlayer,
            };
            // await UniTask.Delay(LOBBIES_REQUEST_COOLDOWN_MILLISECONDS_COUNT);
            lobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id, joinLobbyByIdOptions);

            OnJoinedLobby.Invoke(CurrentLobby);

            Debug.Log($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Completed" +
                                $"\n{nameof(lobby)} == {JsonConvert.SerializeObject(lobby)}");

            return lobby;
        }

        private async UniTask UpdateLobbyList()
        {
            // TODO: add same update logic for CurrentLobby?
            var newLobbies = await GetLobbiesAsync();
            var isLobbiesChanged = !newLobbies.ValueEquals(LobbyList);
            if (!isLobbiesChanged)
            {
                return;
            }

            LobbyList = newLobbies;
        }

        public async UniTask LeaveLobbiesAsync(params string[] lobbyIds)
        {
            Debug.Log($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Started" +
                                $"\n{nameof(lobbyIds)} = {JsonConvert.SerializeObject(lobbyIds)}");

            if (lobbyIds == null ||
                lobbyIds.Length == 0)
            {
                Debug.LogWarning($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Aborted" +
                                           $"\n{nameof(lobbyIds)} = {lobbyIds}");

                return;
            }

            try
            {
                foreach (var lobbyId in lobbyIds)
                {
                    // await UniTask.Delay(LOBBIES_REQUEST_COOLDOWN_MILLISECONDS_COUNT);
                    await LobbyService.Instance.RemovePlayerAsync(lobbyId, AuthenticationManager.Instance.PlayerId);
                }
            }
            catch (LobbyServiceException lobbyServiceException)
            {
                Debug.LogError($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Failed" +
                                         $"\n{nameof(lobbyServiceException)}.{nameof(lobbyServiceException.Reason)} = {lobbyServiceException.Reason}");

                return;
            }

            Debug.Log($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Completed" +
                                $"\n{nameof(lobbyIds)} = {JsonConvert.SerializeObject(lobbyIds)}");
        }
        public async UniTask LeaveLobbiesAsync(List<string> lobbyIds)
        {
            Debug.Log($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Started" +
                                $"\n{nameof(lobbyIds)} = {JsonConvert.SerializeObject(lobbyIds)}");

            await LeaveLobbiesAsync(lobbyIds?.ToArray());

            Debug.Log($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Completed" +
                                $"\n{nameof(lobbyIds)} = {JsonConvert.SerializeObject(lobbyIds)}");
        }
        public async UniTask LeaveLobbiesAsync(params Lobby[] lobbies)
        {
            Debug.Log($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Started" +
                                $"\n{nameof(lobbies)} = {JsonConvert.SerializeObject(lobbies)}");

            var lobbyIds = new List<string>();
            foreach (var lobby in lobbies)
            {
                lobbyIds.Add(lobby.Id);
            }

            await LeaveLobbiesAsync(lobbyIds.ToArray());

            Debug.Log($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Completed" +
                                $"\n{nameof(lobbyIds)} = {JsonConvert.SerializeObject(lobbyIds)}");
        }
        public async UniTask LeaveLobbiesAsync(List<Lobby> lobbies)
        {
            Debug.Log($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Started" +
                                $"\n{nameof(lobbies)} = {JsonConvert.SerializeObject(lobbies)}");

            await LeaveLobbiesAsync(lobbies?.ToArray());

            Debug.Log($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Completed" +
                                $"\n{nameof(lobbies)} = {JsonConvert.SerializeObject(lobbies)}");
        }
        private async UniTask LeaveJoinedLobbiesAsync()
        {
            Debug.Log($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Started");

            JoinedLobbyIds = await GetJoinedLobbyIdsAsync();
            await LeaveLobbiesAsync(JoinedLobbyIds);
            JoinedLobbyIds = null;

            Debug.Log($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Completed");
        }

        public async UniTask StartGame()
        {
            Debug.Log($"{this.GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Started");

            if (CurrentLobby == null) // !IsInLobby
            {
                Debug.LogWarning($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Aborted" +
                                           $"\n{nameof(IsInLobby)} = {IsInLobby}");

                return;
            }

            if (IsInLobby &&
                !IsLobbyHost)
            {
                Debug.LogWarning($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                                         $"\n{nameof(IsLobbyHost)} = {IsLobbyHost}");

                await RelayManager.Instance.JoinRelay(CurrentLobbyRelayJoinCode);

                return;
            }

            try
            {
                await UniTask.WaitUntil(() => RelayManager.Instance != null &&
                                              RelayManager.Instance.IsInitialized);

                var relayJoinCode = await RelayManager.Instance.CreateRelay();
                var updateLobbyOptions = new UpdateLobbyOptions()
                {
                    Data = new Dictionary<string, DataObject>()
                    {
                        {
                            KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode)
                        },
                    }
                };
                var lobby = await Lobbies.Instance.UpdateLobbyAsync(CurrentLobby.Id, updateLobbyOptions);

                CurrentLobby = lobby;
            }
            catch (LobbyServiceException lobbyServiceException)
            {
                Debug.LogError($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}" +
                                         $"\n{nameof(lobbyServiceException)} = {lobbyServiceException}");

                return;
            }

            Debug.Log($"{GetType().Name}.{ReflectionHelper.GetCallerMemberName()}_Completed");
        }

        private void ResetTimer()
        {
            _lobbyPoolTimer = LOBBY_POOL_TIMER_MAX;
        }
        private void UpdateTimer()
        {
            _lobbyPoolTimer -= Time.deltaTime;
        }

        private IEnumerator UpdateLobbyListCoroutine()
        {
            while (IsInitialized)
            {
                yield return UpdateLobbyList();

                yield return new WaitForSeconds(LOBBIES_REQUEST_COOLDOWN_SECONDS_COUNT);
            }

            yield return null;
        }
    }
    public class PlayerCharacter
    {
    }
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GameMode
    {
        DeathMatch,
    }
}
