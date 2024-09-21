using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace Mono.Tetris.Game.Infrastructure;

public class SignalRClient
{
    private HubConnection _connection;

    public SignalRClient(string serverUrl)
    {
        _connection = new HubConnectionBuilder()
            .WithUrl(serverUrl)
            .Build();
    }

    public async Task ConnectAsync()
    {
        await _connection.StartAsync();
    }

    public async void SendPlayerName(string playerName)
    {
        await _connection.InvokeAsync("AddToLobby", playerName);
    }

    public void OnMatchFound(Action callback)
    {
        _connection.On("StartGame", callback);  // Recevoir le signal du serveur pour démarrer le jeu
    }
}
