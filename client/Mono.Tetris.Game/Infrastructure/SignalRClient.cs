using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Mono.Tetris.Engine;

namespace Mono.Tetris.Game.Infrastructure;

public class SignalRClient
{
    private HubConnection _connection;

    private Action<string, string>? _onMoveReceived;

    public SignalRClient(string serverUrl)
    {
        _connection = new HubConnectionBuilder()
            .WithUrl(serverUrl)
            .Build();
    }

    public async Task ConnectAsync()
    {
        _connection.On<string, string>("ReceiveMove", (playerName, move) =>
        {
            _onMoveReceived?.Invoke(playerName, move);
        });

        await _connection.StartAsync();
    }

    public async void SendPlayerName(string playerName)
    {
        await _connection.InvokeAsync("AddToLobby", playerName);
    }

    public async void SendPlayerReady(string playerName)
    {
        await _connection.InvokeAsync("PlayerReady", playerName);
    }
    
    public void OnStartGame(Action<string> onStartGame)
    {
        _connection.On("StartGame", onStartGame);
    }

    public async Task SendMove(string move)
    {
        await _connection.InvokeAsync("SendMove", move);
    }

    public void OnMoveReceived(Action<string, string> onMoveReceived)
    {
        _onMoveReceived = onMoveReceived;
    }

    public async Task SendAddTetromino(string playerName, Tetromino tetromino)
    {
        await _connection.InvokeAsync("AddTetromino", playerName, new TetrominoDto() {
            Name = tetromino.Name
        });
    }

    public void OnTetrominoReceived(Action<string, TetrominoDto> onTetrominoReceived)
    {
        _connection.On("ReceiveTetromino", onTetrominoReceived);
    }

    public void OnStartMatch(Action onStartMatch)
    {
        _connection.On("StartMatch", onStartMatch);
    }
}

public class TetrominoDto
{
    public string Name { get; set; }
}