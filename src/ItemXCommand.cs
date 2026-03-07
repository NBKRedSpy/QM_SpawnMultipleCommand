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
            //The game supports counts now.  The only purpose of this mod now is for autocomplete.
            try
            {
                string itemId = tokens.First();

                if(string.IsNullOrEmpty(itemId)) {
                    return "Please provide an Item ID";
                }

                if(!ItemExists(itemId)) {
                    return $"Item with ID '{itemId}' does not exist.";
                }

                return SpawnItemCommand.Execute(tokens.ToList());
            }
            catch (Exception ex)
            {
                return $"Error executing command.  Generally this is due to the item id not being correct.  Error: {ex.Message}";
            }
        }

        private static bool ItemExists(string itemId)
        {
            return Data.Items._records.Values.Any(x => x.Id.Equals(itemId, StringComparison.OrdinalIgnoreCase));
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

