using UnityEngine;
using UnityEngine.SceneManagement;

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
    public static string joinSecret = "MTI4NzM0OjFpMmhuZToxMjMxMjM=";
    public static string matchSecret = "MmhuZToxMjMxMjM6cWl3amR3MWlqZA==";
    public static string partyId = "ae488379-351d-4a4f-ad32-2b9b01c91657";
    public const int partyMax = 20;
    public static int partySize = 1;

    private DiscordRpc.EventHandlers handlers;
    
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
        DontDestroyOnLoad(this.gameObject);
        presence.largeImageKey = "logo";
        presence.details = "Unknown mode";
        presence.state = "Unknown state";
        presence.partyMax = 20;
        DiscordRpc.UpdatePresence(presence);
        DiscordRpc.RunCallbacks();
    }

    private void Update()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "main":
                details = "In the main menu";
                state = "";
                break;
            case "game":
                details = "In-game";
                state = "5 players left";
                break;
            default:
                details = "Unknown mode";
                state = "Unknown state";
                break;
        }
        if (presence.details != details || presence.state != state || presence.joinSecret != joinSecret || presence.partySize != partySize)
        {
            presence.details = details;
            presence.state = state;
            presence.joinSecret = joinSecret;
            presence.matchSecret = matchSecret;
            presence.partyId = partyId;
            presence.partySize = partySize;
            DiscordRpc.UpdatePresence(presence);
            DiscordRpc.RunCallbacks();
            Debug.Log("Updated RPC");
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