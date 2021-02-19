using UnityEngine;
using UnityEngine.SceneManagement;
using Photon;
using Photon.Pun;
using SajberRoyale.Game;

[System.Serializable]
public class DiscordJoinEvent : UnityEngine.Events.UnityEvent<string> { }

[System.Serializable]
public class DiscordSpectateEvent : UnityEngine.Events.UnityEvent<string> { }

[System.Serializable]
public class DiscordJoinRequestEvent : UnityEngine.Events.UnityEvent<DiscordRpc.DiscordUser> { }

public class DiscordController : MonoBehaviour
{
    public DiscordRpc.RichPresence presence = new DiscordRpc.RichPresence();
    public string applicationId;
    public string optionalSteamId;
    public DiscordRpc.DiscordUser joinRequest;
    public UnityEngine.Events.UnityEvent onConnect;
    public UnityEngine.Events.UnityEvent onDisconnect;
    public UnityEngine.Events.UnityEvent hasResponded;
    public DiscordJoinEvent onJoin;
    public DiscordJoinEvent onSpectate;
    public DiscordJoinRequestEvent onJoinRequest;

    public static string state = "Unknown state";
    public static string details = "Unknown details";
    public static long startTimestamp = 0;
    public static string largeImageText = "Unknown room";

    private DiscordRpc.EventHandlers handlers;

    public static DiscordController Instance;

    public void RequestRespondYes()
    {
        Debug.Log("RPC: responding yes to Ask to Join request");
        DiscordRpc.Respond(joinRequest.userId, DiscordRpc.Reply.Yes);
        hasResponded.Invoke();
    }

    public void RequestRespondNo()
    {
        Debug.Log("RPC: responding no to Ask to Join request");
        DiscordRpc.Respond(joinRequest.userId, DiscordRpc.Reply.No);
        hasResponded.Invoke();
    }

    public void ReadyCallback(ref DiscordRpc.DiscordUser connectedUser)
    {
        Debug.Log(string.Format("RPC: connected to {0}", connectedUser.userId));
        onConnect.Invoke();
    }

    public void DisconnectedCallback(int errorCode, string message)
    {
        Debug.Log(string.Format("RPC: disconnect {0}: {1}", errorCode, message));
        onDisconnect.Invoke();
    }

    public void ErrorCallback(int errorCode, string message)
    {
        Debug.Log(string.Format("RPC: error {0}: {1}", errorCode, message));
    }

    public void JoinCallback(string secret)
    {
        Debug.Log(string.Format("RPC: join ({0})", secret));
        onJoin.Invoke(secret);
    }

    public void SpectateCallback(string secret)
    {
        Debug.Log(string.Format("RPC: spectate ({0})", secret));
        onSpectate.Invoke(secret);
    }

    public void RequestCallback(ref DiscordRpc.DiscordUser request)
    {
        Debug.Log(string.Format("RPC: join request {0}#{1}: {2}", request.username, request.discriminator, request.userId));
        joinRequest = request;
        onJoinRequest.Invoke(request);
    }

    private void Start()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        if (PlayerPrefs.GetInt(Helper.Settings.discordRpc.ToString(), 1) == 1)
        {
            StartRPC();
        }
        
    }
    public void StartRPC()
    {
        presence.largeImageKey = "logo";
        presence.details = "Unknown mode";
        presence.state = "Unknown state";
        DiscordRpc.UpdatePresence(presence);
        DiscordRpc.RunCallbacks();
    }

    private void Update()
    {
        if (PlayerPrefs.GetInt(Helper.Settings.discordRpc.ToString(), 1) != 1) return;
            switch (SceneManager.GetActiveScene().name)
        {
            case "main":
                details = "In the main menu";
                state = "Let's go!";
                largeImageText = "No room";
                break;

            case "game":
                SetGamePresence();
                break;

            default:
                details = "Unknown mode";
                state = "Unknown state";
                largeImageText = "Unknown room";
                break;
        }
        if (presence.details != details || presence.state != state || presence.startTimestamp != startTimestamp)
        {
            presence.details = details;
            presence.state = state;
            presence.startTimestamp = startTimestamp;
            DiscordRpc.UpdatePresence(presence);
        }
        DiscordRpc.RunCallbacks();
    }

    private void SetGamePresence()
    {
        if (Game.Instance.IsActive && PhotonNetwork.IsConnected)
        {
            details = "In-game";
            state = $"{Game.Instance.AlivePlayers}/{Game.Instance.TotalPlayers} player{(Game.Instance.TotalPlayers > 1 ? "s" : "")} left";
            startTimestamp = Game.Instance.StartEpoch;
            largeImageText = $"Room: {PhotonNetwork.CurrentRoom.Name}";
            presence.smallImageKey = Game.Instance.Skin.ToLower();
        }
        else
        {
            details = "Waiting for game";
            if (PhotonNetwork.CurrentRoom == null) return;
            state = $"{PhotonNetwork.CurrentRoom.PlayerCount} player{(PhotonNetwork.CurrentRoom.PlayerCount>1 ? "s" :"")}";
            startTimestamp = 0;
            largeImageText = $"Room: {PhotonNetwork.CurrentRoom.Name}";
        }
    }

    private void OnEnable()
    {
        Debug.Log("RPC: init");
        handlers = new DiscordRpc.EventHandlers();
        handlers.readyCallback += ReadyCallback;
        handlers.disconnectedCallback += DisconnectedCallback;
        handlers.errorCallback += ErrorCallback;
        handlers.joinCallback += JoinCallback;
        handlers.spectateCallback += SpectateCallback;
        handlers.requestCallback += RequestCallback;
        DiscordRpc.Initialize(applicationId, ref handlers, true, optionalSteamId);
    }

    private void OnDisable()
    {
        Debug.Log("RPC: shutdown");
        DiscordRpc.Shutdown();
    }

    private void OnDestroy()
    {
    }
}