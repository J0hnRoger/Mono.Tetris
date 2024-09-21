using Microsoft.AspNetCore.SignalR;

namespace Tetris.Server.Hubs;

public class GameHub : Hub
{
    private static List<string> _players = new();
    private static Dictionary<string, string> _playerConnections = new();
    private readonly ILogger<GameHub> _logger;

    public GameHub(ILogger<GameHub> logger)
    {
        _logger = logger;
    }

    public async Task AddToLobby(string playerName)
    {
        _players.Add(playerName);
        _playerConnections.Add(Context.ConnectionId, playerName);
        _logger.LogInformation($"Player {playerName} connected at {DateTime.Now}.");

        if (_players.Count == 2)
        {
            _logger.LogInformation("Two players connected. Starting the game.");
            await Clients.All.SendAsync("StartGame");
        }
    }

    public async Task SendMove(string move)
    {
        var playerName = _playerConnections[Context.ConnectionId];
        await Clients.Others.SendAsync("ReceiveMove", playerName, move);
    }

    public async Task AddTetromino(string playerName, Tetromino tetromino)
    {
        // Envoyer les informations du Tetromino à l'autre joueur
        await Clients.Others.SendAsync("ReceiveTetromino", playerName, tetromino);
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var playerName = _playerConnections[Context.ConnectionId];
        _players.Remove(playerName);
        _playerConnections.Remove(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
}

public record Tetromino
{
    public string Name { get; set; }
}