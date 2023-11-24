using System;
using Codebase.Runtime.Networking.Client;
using Codebase.Runtime.Networking.Host;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Codebase.Runtime.UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _ipInputField;
        [SerializeField] private TextMeshProUGUI _findMatchButton;
        [SerializeField] private TextMeshProUGUI _queueStatusText;
        [SerializeField] private TextMeshProUGUI _queueTimerText;

        private bool isMatchmaking;
        private bool isCancelling;
        private bool isBusy;
        private float timeInQueue;

        private void Start()
        {
            if (ClientSingleton.Instance == null)
            {
                return;
            }

            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

            _queueStatusText.text = string.Empty;
            _queueTimerText.text = string.Empty;
        }

        private void Update()
        {
            if (isMatchmaking)
            {
                timeInQueue += Time.deltaTime;
                TimeSpan ts = TimeSpan.FromSeconds(timeInQueue);
                _queueTimerText.text = string.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);
            }
        }

        public async void FindMatchPressed()
        {
            if (isCancelling)
            {
                return;
            }

            if (isMatchmaking)
            {
                _queueStatusText.text = "Cancelling...";
                isCancelling = true;
                await ClientSingleton.Instance.GameManager.CancelMatchmaking();
                isCancelling = false;
                isMatchmaking = false;
                isBusy = false;
                _findMatchButton.text = "Find Match";
                _queueStatusText.text = string.Empty;
                _queueTimerText.text = string.Empty;
                return;
            }

            if (isBusy)
            {
                return;
            }

            ClientSingleton.Instance.GameManager.MatchmakeAsync(true, OnMatchMade);
            _findMatchButton.text = "Cancel";
            _queueStatusText.text = "Searching...";
            timeInQueue = 0f;
            isMatchmaking = true;
            isBusy = true;
        }

        private void OnMatchMade(MatchmakerPollingResult result)
        {
            switch (result)
            {
                case MatchmakerPollingResult.Success:
                    _queueStatusText.text = "Connecting...";
                    break;
                case MatchmakerPollingResult.TicketCreationError:
                    _queueTimerText.text = "TicketCreationError";
                    break;
                case MatchmakerPollingResult.TicketCancellationError:
                    _queueTimerText.text = "TicketCancellationError";
                    break;
                case MatchmakerPollingResult.TicketRetrievalError:
                    _queueTimerText.text = "TicketRetrievalError";
                    break;
                case MatchmakerPollingResult.MatchAssignmentError:
                    _queueTimerText.text = "MatchAssignmentError";
                    break;
            }
        }

        public async void StartHost()
        {
            if (isBusy)
            {
                return;
            }

            isBusy = true;

            await HostSingleton.Instance.GameManager.StartHostAsync(true);

            isBusy = false;
        }

        public async void StartClient()
        {
            if (isBusy)
            {
                return;
            }

            isBusy = true;

            await ClientSingleton.Instance.GameManager.StartClientAsync(_ipInputField.text);

            isBusy = false;
        }

        public async void JoinAsync(Lobby lobby)
        {
            if (isBusy)
            {
                return;
            }

            isBusy = true;

            try
            {
                Lobby joiningLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);
                string joinCode = joiningLobby.Data["JoinCode"].Value;

                await ClientSingleton.Instance.GameManager.StartClientAsync(joinCode);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }

            isBusy = false;
        }
    }
}