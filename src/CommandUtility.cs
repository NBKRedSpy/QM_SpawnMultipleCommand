using MGSC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QM_SpawnMultipleCommand
{
    public class CommandUtility
    {
        public void InjectCommand(Type commandType, string name)
        {

            ConsoleDaemon consoleDaemon = ConsoleDaemon.Instance;

            if (consoleDaemon == null) return;


            MethodInfo helpMethod = commandType.GetMethod("Help", BindingFlags.Static | BindingFlags.Public, null, CallingConventions.Standard, new Type[2]
            {
                typeof(string),
                typeof(bool)
            }, null);

            MethodInfo fetchAutocompleteMethod = commandType.GetMethod("FetchAutocompleteOptions", BindingFlags.Static | BindingFlags.Public, null, CallingConventions.Standard, new Type[2]
{
                typeof(string),
                typeof(string[])
}, null);

            if (helpMethod == null && fetchAutocompleteMethod == null) return;

            MethodInfo isAvailableMethod = commandType.GetMethod("IsAvailable", BindingFlags.Static | BindingFlags.Public, null, CallingConventions.Standard, new Type[0], null);
            MethodInfo showInHelpAndAutocompleteMethod = commandType.GetMethod("ShowInHelpAndAutocomplete", BindingFlags.Static | BindingFlags.Public, null, CallingConventions.Standard, new Type[0], null);



            consoleDaemon.Commands[name.ToLower()] = new ConsoleDaemon.CommandInterface(
                commandType, consoleDaemon.Resolve, helpMethod, fetchAutocompleteMethod,
                isAvailableMethod, showInHelpAndAutocompleteMethod);

            //Seems to always set this, even without alts.
            //Oddly does not do a ToLower
            consoleDaemon.AlternateCommandNames[name] = name;
        }

    }
}

