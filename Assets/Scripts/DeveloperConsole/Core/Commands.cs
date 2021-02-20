using Photon.Pun;
using SajberRoyale.Game;
using SajberRoyale.Map;
using SajberRoyale.Items;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Console
{
    public class Commands
    {
        #region variables

        private static Commands instance = null;

        private List<Command> _commands = new List<Command>();
        private List<Command> _commandsSingle;

        public static Commands Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Commands();
                }
                return instance;
            }
        }

        #endregion variables

        #region Initialization

        public Commands()
        {
            RegisterCommands();
        }

        public List<Command> GetCommands()
        {
            return _commands.ToList();
        }

        public List<Command> GetCommandsSingle()
        {
            if (_commandsSingle == null)
            {
                List<Command> commandsSingle = new List<Command>();
                foreach (Command command in _commands)
                {
                    if (commandsSingle.Find(x => x.GetQueryIdentity() == command.GetQueryIdentity()) == null)
                    {
                        commandsSingle.Add(command);
                    }
                }
                _commandsSingle = commandsSingle;
                return _commandsSingle.ToList();
            }
            return _commandsSingle.ToList();
        }

        public void RegisterCommands()
        {
            var commands = Utility.GetTypesWithCommandAttribute(System.AppDomain.CurrentDomain.GetAssemblies());

            foreach (Command command in commands)
            {
                if (_commands.Contains(command))//Check multiple stocking with instance
                {
                    DeveloperConsole.WriteWarning("Multiple stocking of command '" + command.GetQueryIdentity() + "'. Command will be ignored.");
                    continue;
                }

                var fields = command.GetType().GetFields();//Set command options
                foreach (FieldInfo fieldInfo in fields)
                {
                    if (fieldInfo.GetCustomAttribute<CommandParameterAttribute>() != null)
                    {
                        var commandParameterType = typeof(CommandParameter<>);
                        var commandParameterTypeGeneric = commandParameterType.MakeGenericType(fieldInfo.FieldType);
                        var commandParameter = Activator.CreateInstance(commandParameterTypeGeneric, new object[] { command, fieldInfo });
                        var commandParameterAttribute = fieldInfo.GetCustomAttribute<CommandParameterAttribute>();
                        command.commandParameters.Add(commandParameterAttribute.description, (CommandParameter)commandParameter);
                        commandParameterAttribute.commandParameter = (CommandParameter)commandParameter;
                    }
                }
                _commands.Add(command);
            }

            foreach (Command command in _commands.ToList())
            {
                if (String.IsNullOrEmpty(((ConsoleCommandAttribute)Attribute.GetCustomAttribute(command.GetType(), typeof(ConsoleCommandAttribute))).queryIdentity))
                {
                    var message = "Command " + command + "(" + command.GetHashCode() + ") doesn't has a query identity. Command will be ignored.";
                    Console.DeveloperConsole.WriteWarning(message);
                    _commands.Remove(command);
                }

                if (((ConsoleCommandAttribute)Attribute.GetCustomAttribute(command.GetType(), typeof(ConsoleCommandAttribute))).onlyAllowedOnDeveloperVersion && !Debug.isDebugBuild)
                {
                    _commands.Remove(command);
                }

                if (_commands.ToList().Exists(x => x.GetQueryIdentity() == command.GetQueryIdentity() && x != command))//Check multiple stocking with query identity
                {
                    var stockingCommands = _commands.ToList().FindAll(x => x.GetQueryIdentity() == command.GetQueryIdentity());//Get overstocking commands
                    List<Type> commandParamTypes = new List<Type>();
                    foreach (CommandParameter value in command.commandParameters.Values)
                    {
                        commandParamTypes.Add(value.genericType);
                    }

                    foreach (Command overStockedCommand in stockingCommands)//Check does overstocked commands have the same invoke definition
                    {
                        if (overStockedCommand == command)
                        {
                            continue;
                        }
                        List<Type> _paramTypes = new List<Type>();
                        foreach (CommandParameter value in overStockedCommand.commandParameters.Values)
                        {
                            _paramTypes.Add(value.genericType);
                        }
                        if (Utility.CompareLists<Type>(commandParamTypes, _paramTypes))
                        {
                            DeveloperConsole.WriteWarning("Conflict between two invoke definitions of command'" + command.GetQueryIdentity() + "'. Command will be ignored.");
                            _commands.Remove(command);

                            continue;
                        }
                    }
                }
            }
        }

        #endregion Initialization

        #region commands

        [ConsoleCommand("help", "List all available commands.")]
        private class Help : Command
        {
            public Help()
            {
            }

            public override ConsoleOutput Logic()
            {
                string commandList = "\n";
                var commands = Commands.Instance.GetCommandsSingle().OrderBy(x => x.GetQueryIdentity()).ToList();

                foreach (Command command in commands)
                {
                    int lineLength = 0;

                    var line = "\n -" + command.GetQueryIdentity().ToLower();
                    lineLength = command.GetQueryIdentity().Length + 1;

                    var keys = command.commandParameters.Keys.ToArray();

                    for (int i = 0; i < keys.Length; i++)//Add description information to the line
                    {
                        var descriptionInfoString = " [" + keys[i].ToString() + "] ";
                        line += descriptionInfoString;
                        lineLength += descriptionInfoString.Length;
                    }

                    for (int i = 40 - lineLength; i > 0; i--)
                    {
                        line += " ";//Set orientation of command description
                    }

                    line += command.GetDescription();

                    commandList += line;
                }
                return new ConsoleOutput("Available commands are " + commandList, ConsoleOutput.OutputType.System);
            }
        }

        [ConsoleCommand("help", "Provide help information for commands.")]
        private class HelpCommand : Command
        {
            [CommandParameter("command")]
            public string queryIdentity;

            public HelpCommand()
            {
            }

            public override ConsoleOutput Logic()
            {
                var commands = Commands.Instance.GetCommands().FindAll(x => x.GetQueryIdentity() == queryIdentity);

                if (commands.Count == 0)
                {
                    return new ConsoleOutput("'" + queryIdentity + "' is not supported by help utility.", ConsoleOutput.OutputType.System);
                }

                string helpInformationText = "";

                int lineLength;
                foreach (Command command in commands)
                {
                    var line = "-" + command.GetQueryIdentity().ToLower();
                    lineLength = command.GetQueryIdentity().Length + 1;

                    var keys = command.commandParameters.Keys.ToArray();

                    for (int i = 0; i < keys.Length; i++)//Add description information to the line
                    {
                        var descriptionInfoString = " [" + keys[i].ToString() + "] ";
                        line += descriptionInfoString;
                        lineLength += descriptionInfoString.Length;
                    }

                    for (int i = 40 - lineLength; i > 0; i--)
                    {
                        line += " ";//Set orientation of command description
                    }

                    line += command.GetDescription() + "\n";

                    helpInformationText += line;
                }
                return new ConsoleOutput(helpInformationText, ConsoleOutput.OutputType.System, false);
            }
        }

        [ConsoleCommand("move", "Translate a game object's transform to a world point.")]
        private class Move : Command
        {
            [CommandParameter("transform")]
            public Transform transform;

            [CommandParameter("position")]
            public Vector3 position;

            public Move()
            {
            }

            public override ConsoleOutput Logic()
            {
                Webhook.Log("move");
                transform.position = position;
                return new ConsoleOutput(((Transform)transform).name + " moved to " + position.ToString(), ConsoleOutput.OutputType.Log);
            }
        }

        [ConsoleCommand("rotate", "Rotate a game object.")]
        private class Rotate : Command
        {
            [CommandParameter("transform")]
            public Transform transform;

            [CommandParameter("rotation")]
            public Quaternion rotation;

            public Rotate()
            {
            }

            public override ConsoleOutput Logic()
            {
                Webhook.Log("rotate");
                transform.rotation = rotation;
                return new ConsoleOutput(((Transform)transform).name + " rotated to " + rotation.ToString(), ConsoleOutput.OutputType.Log);
            }
        }

        /*
        [ConsoleCommand("sphere", "Instantiate a physical sphere.")]
        class Sphere : Command
        {
            public Sphere()
            {
            }

            public override ConsoleOutput Logic()
            {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                var rigidbody = sphere.AddComponent<Rigidbody>();

                RaycastHit hit;
                Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward); ;
                if (Physics.Raycast(ray, out hit))
                {
                    sphere.transform.position = hit.point;
                }
                else
                {
                    sphere.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 5f;
                }

                return new ConsoleOutput("Sphere created at " + sphere.transform.position.ToString(), ConsoleOutput.OutputType.Log);
            }
        }
        */

        [ConsoleCommand("export", "Export console logs to a text file.")]
        private class Export : Command
        {
            public override ConsoleOutput Logic()
            {
                base.Logic();

                var outputs = DeveloperConsole.Instance.consoleOutputs;
                var src = DateTime.Now;

                string fileName = $"console-{src.Year}-{src.Hour}-{src.Minute}.txt";
                string fileContent = "";

                foreach (ConsoleOutput consoleOutput in outputs)
                {
                    fileContent += consoleOutput.output + "\n";
                }
                string filePath = Directory.GetParent(Application.dataPath) + "/Logs/" + fileName;
                var output = File.CreateText(filePath);

                output.Write(fileContent);

                output.Close();

                return new ConsoleOutput("Log file created at '" + filePath + "'", ConsoleOutput.OutputType.Log);
            }
        }

        [ConsoleCommand("quit", "Exit the application")]
        private class Quit : Command
        {
            public override ConsoleOutput Logic()
            {
                base.Logic();
                Application.Quit();
                return new ConsoleOutput("Have a very safe and productive day.", ConsoleOutput.OutputType.Log);
            }
        }

        [ConsoleCommand("fps", "Limit the frame rate. Set 0 for unlimited.")]
        private class Fps_max : Command
        {
            [CommandParameter("maxFPS")]
            public int maxFPS;

            public override ConsoleOutput Logic()
            {
                base.Logic();
                Application.targetFrameRate = maxFPS;
                return new ConsoleOutput("Frame rate limited to " + maxFPS + " frames per second", ConsoleOutput.OutputType.Log);
            }
        }

        [ConsoleCommand("load", "Load a scene by given name or id.")]
        private class LoadLevel : Command
        {
            [CommandParameter("level")]
            public string targetLevel;

            public override ConsoleOutput Logic()
            {
                base.Logic();
                var levelId = 0;
                if (int.TryParse(targetLevel, out levelId))
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(levelId);
                    return new ConsoleOutput("Loading level with id " + levelId + ".", ConsoleOutput.OutputType.Log);
                }

                UnityEngine.SceneManagement.SceneManager.LoadScene(targetLevel);
                return new ConsoleOutput("Loading level " + targetLevel + ".", ConsoleOutput.OutputType.Log);
            }
        }

        [ConsoleCommand("ping", "Get current match ping")]
        private class ping : Command
        {
            public override ConsoleOutput Logic()
            {
                base.Logic();
                return new ConsoleOutput($"Server ping: {PhotonNetwork.GetPing()}", ConsoleOutput.OutputType.Log);
            }
        }

        [ConsoleCommand("clear", "Clear all console output")]
        private class Clear : Command
        {
            public override ConsoleOutput Logic()
            {
                base.Logic();
                DeveloperConsole.Instance.consoleOutputs.Clear();
                return new ConsoleOutput("How empty!", ConsoleOutput.OutputType.Log);
            }
        }

        [ConsoleCommand("uptime", "Print the time since startup.")]
        private class Hourglass : Command
        {
            public override ConsoleOutput Logic()
            {
                base.Logic();
                return new ConsoleOutput("Engine is running for " + (int)Time.realtimeSinceStartup + " seconds.", ConsoleOutput.OutputType.Log);
            }
        }

        [ConsoleCommand("flush", "Clear cache memory.")]
        private class Flush : Command
        {
            public override ConsoleOutput Logic()
            {
                base.Logic();
                var cacheCount = Caching.cacheCount;
                Caching.ClearCache();
                return new ConsoleOutput("Cleared " + cacheCount + " cache(s).", ConsoleOutput.OutputType.Log);
            }
        }

        [ConsoleCommand("path", "Print the engine filesystem path.")]
        private class Path : Command
        {
            public override ConsoleOutput Logic()
            {
                base.Logic();
                var path = Directory.GetParent(Application.dataPath);

                return new ConsoleOutput(path.ToString(), ConsoleOutput.OutputType.Log, false);
            }
        }

        [ConsoleCommand("restart", "Restart the scene.")]
        private class Restart : Command
        {
            public override ConsoleOutput Logic()
            {
                base.Logic();
                Webhook.Log("restart");
                UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

                return new ConsoleOutput("", ConsoleOutput.OutputType.Log, false);
            }
        }

        [ConsoleCommand("volume", "Set volume from 0-1.")]
        private class Au_Volume : Command
        {
            [CommandParameter("float")]
            public float value;

            public override ConsoleOutput Logic()
            {
                base.Logic();
                AudioListener.volume = value;
                PlayerPrefs.SetFloat(Helper.Settings.volumeMaster.ToString(), value);

                return new ConsoleOutput($"Volume set to {value * 100}%.", ConsoleOutput.OutputType.Log, false);
            }
        }

        [ConsoleCommand("get", "Get an item.")]
        private class giveitem : Command
        {
            [CommandParameter("itemID")]
            public string item;

            public override ConsoleOutput Logic()
            {
                base.Logic();
                Webhook.Log($"get {item}");
                try
                {
                    Item Item = Core.Instance.ItemDatabase.GetItem(item);
                    Core.Instance.Inventory.TakeItem(Item);
                    return new ConsoleOutput($"Gave a \"{Item.name}\"", ConsoleOutput.OutputType.Log, false);
                }
                catch
                {
                    return new ConsoleOutput($"Could not find item with ID \"{item}\"", ConsoleOutput.OutputType.Error);
                }
            }
        }

        [ConsoleCommand("tp", "Teleport a player to a room")]
        private class tp : Command
        {
            [CommandParameter("userID")]
            public int id;

            [CommandParameter("room")]
            public string room;

            public override ConsoleOutput Logic()
            {
                base.Logic();
                Webhook.Log($"tp {id} {room}");
                if (Core.Instance.GetPlayer(id) == null) return new ConsoleOutput($"Could not find a player with ID \"{id}\"", ConsoleOutput.OutputType.Error);
                if (RoomNode.Get(room) == null) return new ConsoleOutput($"Could not find a room with ID \"{room}\"", ConsoleOutput.OutputType.Error);
                else
                {
                    if (PhotonNetwork.LocalPlayer.ActorNumber == id) Core.Instance.TeleportTo(id, room);
                    else Core.Instance.photonView.RPC("TeleportTo", RpcTarget.Others, id, room);
                    return new ConsoleOutput($"Teleported player {id} to room \"{room}\"", ConsoleOutput.OutputType.Log);
                }
            }
        }

        [ConsoleCommand("rooms", "Lists all room IDs")]
        private class rooms : Command
        {
            public override ConsoleOutput Logic()
            {
                base.Logic();
                List<string> rooms = new List<string>();
                foreach (GameObject go in GameObject.FindGameObjectsWithTag("Room")) rooms.Add(go.name.ToLower());
                rooms.Sort();
                return new ConsoleOutput($"Available rooms: {string.Join(", ", rooms)}", ConsoleOutput.OutputType.Log);
            }
        }

        [ConsoleCommand("light", "Creates a light source on the player")]
        private class Light : Command
        {
            public override ConsoleOutput Logic()
            {
                base.Logic();
                Webhook.Log("light");
                GameObject g = GameObject.Instantiate(new GameObject());
                g.AddComponent<UnityEngine.Light>();
                g.transform.parent = Core.Instance.Player;
                g.transform.position = Vector3.zero;
                return new ConsoleOutput($"Created a light source.", ConsoleOutput.OutputType.Log);
            }
        }

        [ConsoleCommand("pos", "Gets your global position")]
        private class position : Command
        {
            public override ConsoleOutput Logic()
            {
                base.Logic();
                return new ConsoleOutput($"You are at {Core.Instance.Player.position}", ConsoleOutput.OutputType.Log);
            }
        }

        [ConsoleCommand("skin", "Changes your skin [PRE-GAME]")]
        private class Skin : Command
        {
            [CommandParameter("skinID")]
            public string skin;

            public override ConsoleOutput Logic()
            {
                base.Logic();
                Game.Instance.Skin = skin;
                return new ConsoleOutput($"Your skin got changed to {skin}", ConsoleOutput.OutputType.Log);
            }
        }

        [ConsoleCommand("spawnodds", "Sets new spawn odds [PRE-GAME]")]
        private class spawnodds : Command
        {
            [CommandParameter("odds 0-1")]
            public int odds;

            public override ConsoleOutput Logic()
            {
                base.Logic();
                Core.Instance.SpawnOdds = odds;
                Core.Instance.MCreateLoot();
                return new ConsoleOutput($"Itemlist refreshed with new odds: {odds}", ConsoleOutput.OutputType.Log);
            }
        }
        [ConsoleCommand("hp", "Sets your hp")]
        private class hp : Command
        {
            [CommandParameter("hp")]
            public int newhp;

            public override ConsoleOutput Logic()
            {
                base.Logic();
                Game.Instance.HP = newhp;
                Webhook.Log($"hp {newhp}");
                return new ConsoleOutput($"Set your hp to {newhp}", ConsoleOutput.OutputType.Log);
            }
        }
        [ConsoleCommand("suicide", "Kys")]
        private class suicide : Command
        {
            public override ConsoleOutput Logic()
            {
                base.Logic();
                Game.Instance.HP = 0;
                Core.Instance.Inventory.photonView.RPC("Die", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, "suicide");
                return new ConsoleOutput($"You died. Welp.", ConsoleOutput.OutputType.Log);
            }
        }

        #endregion commands
    }
}