using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NCmd;

namespace HaasToolTemplate
{
    class InteractiveShell : Cmd
    {
        private const string HistFileName = "ppscreener-cmd";

        [CmdCommand(Command = "exit", Description = StaticStrings.EXIT_HELP_TEXT)]
        public void ExitCommand(string arg)
        {
            ExitLoop();
        }

        [CmdCommand(Command = "clear", Description = StaticStrings.CLEAR_HELP_TEXT)]
        public void ClearCommand(string arg)
        {
            Console.Clear();
        }

        [CmdCommand(Command = "version", Description = StaticStrings.VERSION_HELP_TEXT)]
        public void ShowVersion(string arg)
        {
            WriteVersionStatement(new AutoProgramMetaData(GetType().Assembly), Console.Out);
        }

        [CmdCommand(Command = "show-config", Description = StaticStrings.SHOW_CONFIG_HELP_TEXT)]
        public void ShowConfigCommand(string arg)
        {
            ActionManager.ShowConfig();
        }

        [CmdCommand(Command ="set-config", Description = StaticStrings.SET_CONFIG_HELP_TEXT)]
        public void SetConfigCommand(string arg)
        {
            string[] arguments = Utils.SplitArgumentsSaftley(arg);

            if (arguments.Length >= 2)
            {
                switch (arguments[0])
                {
                    case "ipaddress":
                        ActionManager.SetConfigIpAddress(arguments[1]);
                        Console.WriteLine("[*] Haas Ip Address Set To {0}", arguments[1]);
                        break;

                    case "port":
                        int dead = 0;
                        if (Int32.TryParse(arguments[1], out dead)) { 
                            ActionManager.SetConfigPort(Convert.ToInt32(arguments[1]));
                            Console.WriteLine("[*] Haas Port Set To {0}", arguments[1]);
                        }
                        else
                        {
                            Console.WriteLine("[!] Argument Is Not A Number");
                        }
                        break;

                    case "secret":
                        ActionManager.SetConfigSecret(arguments[1]);
                        Console.WriteLine("[*] Haas Secret Set To {0}", arguments[1]);
                        break;

                    case "accountguid":
                        ActionManager.SetConfigSecret(arguments[1]);
                        Console.WriteLine("[*] Haas Account GUID Set To {0}", arguments[1]);
                        break;

                    default:
                        Console.WriteLine("Argument not valid");
                        break;

                }

            }
            else
            {
                Console.WriteLine("[!] Not Enough Arguments Specified");
                Console.WriteLine("Ex. set-config <configOption> <value>");
            }

        }

        [CmdCommand(Command = "save-config", Description = StaticStrings.SAVE_CONFIG_HELP_TEXT)]
        public void SaveConfigCommand(string arg)
        {
            string[] arguments = Utils.SplitArgumentsSaftley(arg);

            if (arguments.Length == 1)
            {
                if(ActionManager.SaveConfig(arguments[0]))
                {
                    Console.WriteLine("[*] Saved Config With Filename {0}", arg[0]);
                }
                else
                {
                    Console.WriteLine("[!] Could Not Save Config {0}", arg[0]);
                }
            }
            else
            {
                if(ActionManager.SaveConfig())
                {
                    Console.WriteLine("[*] Saved Default Config File {0}", ActionManager.DefaultConfigName);
                }
                else
                {
                    Console.WriteLine("[!] Could Not Save Default Config File {0}", arg[0]);
                }
            }
        }

        [CmdCommand(Command = "load-config", Description = StaticStrings.LOAD_CONFIG_HELP_TEXT)]
        public void LoadConfigCommand(string arg)
        {
            string[] arguments = Utils.SplitArgumentsSaftley(arg);

            if (arguments.Length == 1)
            {
                if(ActionManager.LoadConfig(arguments[0]))
                {
                    Console.WriteLine("[*] Loaded Config With Filename {0}", arg[0]);
                } else
                {
                    Console.WriteLine("[!] Could Not Load Config {0}", arg[0]);
                }
            }
            else
            {
                if (ActionManager.LoadConfig())
                {
                    Console.WriteLine("[*] Loaded Default Config File {0}", ActionManager.DefaultConfigName);
                }
                else
                {
                    Console.WriteLine("[!] Could Not Load Default Config File {0}", arg[0]);
                }
            }
        }

        [CmdCommand(Command = "test-creds", Description = StaticStrings.TEST_CREDS_HELP_TEXT)]
        public void TestCredsCommand(string arg)
        {
            Console.WriteLine("[*] Verifying API Connection and Credentials");

            if (ActionManager.CheckHaasConnection())
            {
                Console.WriteLine("[*] Connection Succesfull");
            }
            else
            {
                Console.WriteLine("[!] Connection Failed Check ip:port/credentials");
            }

        }

        [CmdCommand(Command = "show-accounts", Description = StaticStrings.SHOW_ACCOUNTS_HELP_TEXT)]
        public void ShowAccountGuidsCommand(string arg)
        {
            int count = 1;

            Console.WriteLine("\n---- Current Active Accounts ----");

            foreach ( var account in ActionManager.GetAccountGUIDS())
            {
                Console.WriteLine("#{0} - {1} : {2}", count, account.Item1, account.Item2);
                count++;
            }

            Console.WriteLine();
            
        }

        [CmdCommand(Command = "set-account", Description = StaticStrings.SET_ACCOUNT_HELP_TEXT)]
        public void SetAccountCommand(string arg)
        {
            string[] arguments = Utils.SplitArgumentsSaftley(arg);

            if (arguments.Length == 1)
            {
                int dead = 0;
                if (Int32.TryParse(arguments[0], out dead))
                {
                    int index = Convert.ToInt32(arguments[0]);
                    var accounts = ActionManager.GetAccountGUIDS();

                    var accountPair = new Tuple<string, string>("","");

                    if (index > accounts.Count || index == 0)
                    {
                        Console.WriteLine("[!] Invalid Account Selection");
                    } 
                    else
                    { 
                        accountPair = ActionManager.GetAccountGUIDS()[Convert.ToInt32(arguments[0])-1];
                        ActionManager.SetAccountGUID(accountPair.Item2);
                        Console.WriteLine("[*] Haas Account Set To {0} : {1}", accountPair.Item1, accountPair.Item2);
                    }

                }
                else
                {
                    Console.WriteLine("[!] Argument Is Not A Number");
                }
            }
            else
            {
                Console.WriteLine("Not Enough Arguments");
                Console.WriteLine("Ex. set-account <account-num>");
            }
        }

        [CmdCommand(Command = "show-markets", Description = StaticStrings.SHOW_MARKETS_HELP_TEXT)]
        public void ShowMarketsCommand(string arg)
        {
            var markets = ActionManager.GetMarkets();

            if (markets.Count == 0)
            {
                Console.WriteLine("[!] Could Not Obtain Market Information");
            }
            else
            {
                Console.WriteLine("\n---- Current Markets ----");
                foreach (string market in markets)
                {
                    Console.WriteLine("{0}/{1}", market, ActionManager.mainConfig.PrimaryCurrency);
                }
            }

        }

        public InteractiveShell()
        {

            Utils.ShowBanner();

            Intro = "";
            CommandPrompt = "$> ";
            HistoryFileName = HistFileName;
        }


        public override void PostCmd(string line)
        {
            base.PostCmd(line);
            //Console.WriteLine();
        }

        public override string PreCmd(string line)
        {
            return base.PreCmd(line);
        }

        public override void EmptyLine()
        {
            Console.WriteLine("Please enter a command or type 'help' for assitance.");
        }

        public override void PreLoop()
        {
            if(ActionManager.PerformStartup())
            {
                Console.WriteLine("[*] Succesfully Loaded Default Config {0} ", ActionManager.DefaultConfigName);
            }
            else
            {
                Console.WriteLine("[!] Failed To Load Default Config {0} ", ActionManager.DefaultConfigName);
                Console.WriteLine("[!] Generated New Config Config {0} ", ActionManager.DefaultConfigName);
                ActionManager.SaveConfig();
            }

        }

        public override void PostLoop()
        {

        }
    }
}
