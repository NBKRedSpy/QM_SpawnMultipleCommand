using MGSC;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace QM_SpawnMultipleCommand
{
    [ConsoleCommand(new string[] { commandName })]
    public class ItemXCommand
    {

        private const string commandName = "itemx";

        /// <summary>
        /// Used to only show the "cannot find game's item command" only once as to not spam it.
        /// </summary>
        private static bool ErrorHasBeenShown { get; set; } = false;

        private static ConsoleDaemon.CommandInterface SpawnItemCommand { get; set; }

        private static void Write(string text)
        {
            UI.Get<DevConsole>().PrintText(text);
        }

        public static string Help(string command, bool verbose)
        {
            //Console does not support multiple line output.  Workaround
            return ("itemx <item> <count>");
        }

        /// <summary>
        /// The project and save to remove the project from.
        /// </summary>
        /// <param name="tokens">Save slot number (0-2) and project id.</param>
        /// <returns></returns>
        public static string Execute(string[] tokens)
        {
            try
            {
                if (tokens.Length != 2)
                {
                    return $"Expected the number of items to drop";
                }

                //The count value.
                if (!int.TryParse(tokens[1], out int count) || count <= 0)
                {
                    return $"The item count must be set and >= 0";
                }

                List<string> tokenList = tokens.ToList();
                tokenList.RemoveAt(tokenList.Count - 1);  //Remove the count

                var messages = new Dictionary<string, int>();

                try
                {
                    for (int i = 0; i < count; i++)
                    {
                        string result = SpawnItemCommand.Execute(tokenList);

                        //The command's expected result.  Currently the command throws an exception 
                        //  and silently fails, but checking just in case it changes.
                        if (result != "done!")
                        {
                            return ("$\"Error executing the item command. '{result}'");
                        }

                        //There should only be the "done!" message, but just in case.
                        if (messages.TryGetValue(result, out int messageCount))
                        {
                            messages[result] = ++messageCount;
                        }
                        else
                        {
                            messages.Add(result, 1);
                        }
                    }

                    foreach (string item in messages.Keys)
                    {
                        //Does not support multiple lines. Use the console directly.
                        Write($"{item} ({messages[item]})");
                        return "";
                    }

                    return "";
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                    return $"Error executing the item command.  Verify the item id is correct. {ex.Message}";
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
                return $"Command failed.  See Player.log for more information.  {ex.Message}";
            }
        }

        /// <summary>
        /// Returns the list of items that partially match the text
        /// </summary>
        /// <param name="command"></param>
        /// <param name="tokens"></param>
        /// <returns></returns>
        public static List<string> FetchAutocompleteOptions(string command, string[] tokens)
        {
            if(tokens.Length == 0) return new List<string>();

            string id = tokens[0].Trim();

            //Todo:  Support localization item name search.
            //  Probably show (foo) and have the command ignore the (foo) part.
            //This is not currently supported by the game, so it is commented out.
            ////Get the list of all the items in the localization that may match.
            //List< KeyValuePair<string, string>> textMatches = 
            //    Localization.Instance.currentDict
            //    .Where(x =>
            //        x.Key.IndexOf(id, StringComparison.OrdinalIgnoreCase) != -1 &&
            //        x.Key.StartsWith("item.") && x.Key.EndsWith(".name"))
            //    .ToList();

            List<string> ids = Data.Items._records.Values
                .Where(x => x.Id.IndexOf(id, StringComparison.OrdinalIgnoreCase) != -1)
                .Select(x => $"{commandName} {x.Id}")
                .ToList();

            return ids;
        }

        public static bool IsAvailable()
        {
            DevConsole devConsole = UI.Get<DevConsole>();

            if (SpawnItemCommand == null)
            {
                if(devConsole.Daemon._commands.TryGetValue("item", out ConsoleDaemon.CommandInterface command))
                {
                    SpawnItemCommand = command;
                }
            }

            if (SpawnItemCommand == null)
            {
                if (!ErrorHasBeenShown)
                {
                    ErrorHasBeenShown = true;
                    devConsole.PrintText("Unable to find the game's Item console command");
                }
                return false;
            }

            return SpawnItemCommand.IsAvailable;
        }

        public static bool ShowInHelpAndAutocomplete()
        {
            return SpawnItemCommand.IsAvailable;
        }
    }
}

