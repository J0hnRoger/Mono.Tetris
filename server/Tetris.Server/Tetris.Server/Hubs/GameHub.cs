using Microsoft.AspNetCore.SignalR;

namespace Tetris.Server.Hubs;

public class GameHub : Hub
{
    private static List<string> _players = new();
    private static Dictionary<string, string> _playerConnections = new();
    private static List<Game> CurrentGames = new();
    private readonly ILogger<GameHub> _logger;
    private readonly bool _devMode;

    public GameHub(ILogger<GameHub> logger, IConfiguration configuration)
    {
        _logger = logger;
        _devMode = configuration.GetValue<bool>("DevMode");
    }

    public async Task AddToLobby(string playerName)
    {
        _players.Add(playerName);
        _playerConnections.Add(Context.ConnectionId, playerName);
        _logger.LogInformation($"Player {playerName} connected at {DateTime.Now}.");
        if (_devMode && _players.Count == 1)
        {
            // Ajout d'un joueur bot
            string botPlayerName = "BotPlayer";
            _players.Add(botPlayerName);
            _playerConnections.Add("bot-connection-id", botPlayerName);

            _logger.LogInformation("Bot player added to the game in development mode.");

            // Ajout du jeu avec le joueur humain et le bot
            CurrentGames.Add(new Game()
            {
                Player1 = new Player() {Name = playerName, ConnectionId = Context.ConnectionId},
                Player2 = new Player() {Name = botPlayerName, ConnectionId = "bot-connection-id"}
            });

            // Informer le client joueur que la partie commence avec le bot
            await Clients.Client(Context.ConnectionId).SendAsync("StartGame", botPlayerName);

            // Commencer à simuler les actions du bot
            await SimulateBotActions();
        }
        else if (_players.Count == 2)
        {
            var player1ConnectionId = _playerConnections.ElementAt(0).Key;
            var player1Name = _playerConnections.ElementAt(0).Value;

            var player2ConnectionId = _playerConnections.ElementAt(1).Key;
            var player2Name = _playerConnections.ElementAt(1).Value;

            _logger.LogInformation("Two players connected. Starting the game.");

            CurrentGames.Add(new Game()
            {
                Player1 = new Player() {Name = player1Name, ConnectionId = player1ConnectionId},
                Player2 = new Player() {Name = player2Name, ConnectionId = player2ConnectionId}
            });

            await Clients.Client(player1ConnectionId).SendAsync("StartGame", player2Name);
            await Clients.Client(player2ConnectionId).SendAsync("StartGame", player1Name);
        }
    }
    
    private async Task SimulateBotActions()
    {
        // TODO
    }


    public async Task PlayerReady(string player)
    {
        var currentGame = CurrentGames.First(g => g.Player2.Name == player || g.Player1.Name == player);
        var currentPlayer = currentGame.Player1.Name == player ? currentGame.Player1 : currentGame.Player2;

        currentPlayer.Ready = true;
        bool isBot = _devMode && currentGame.Player1.Ready;
        if ((currentGame.Player1.Ready && currentGame.Player2.Ready)|| isBot)
        {
            await Clients.Client(currentGame.Player1.ConnectionId).SendAsync("StartMatch");
            await Clients.Client(currentGame.Player2.ConnectionId).SendAsync("StartMatch");
        }
    }

    public async Task SendMove(string move)
    {
        var playerName = _playerConnections[Context.ConnectionId];
        await Clients.Others.SendAsync("ReceiveMove", playerName, move);
    }

    public async Task AddTetromino(string playerName, Tetromino tetromino)
    {
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

internal class Game
{
    public Player Player1 { get; set; }
    public Player Player2 { get; set; }
}

internal class Player
{
    public string Name { get; set; }
    public string ConnectionId { get; set; }
    public bool Ready { get; set; }
}

public record Tetromino
{
    public string Name { get; set; }
}