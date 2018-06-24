using Haasonline.Public.LocalApi.CSharp;
using Haasonline.Public.LocalApi.CSharp.DataObjects.AccountData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace HaasToolTemplate
{
    // Meat of the application
    // Performs all real actions
    public static class ActionManager
    {
        public static HaasConfig mainConfig;

        public static string DefaultConfigName = "HaasTool.config";

        public static bool LoadConfig()
        {

            if (File.Exists(DefaultConfigName))
            {
                ActionManager.mainConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<HaasConfig>(File.ReadAllText(DefaultConfigName));
                return true;
            }
            else
            {
                return false;
            }

        }

        public static bool LoadConfig(string fileName)
        {

            if (File.Exists(fileName))
            {
                ActionManager.mainConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<HaasConfig>(File.ReadAllText(fileName));
                return true;
            }
            else
            {
                return false;
            }

        }

        public static bool SaveConfig()
        {
            try
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(ActionManager.mainConfig);

                File.WriteAllText(DefaultConfigName, json);

                return true;
            }
            catch
            {

                return false;
            }

        }
 
        public static bool SaveConfig(string fileName)
        {
            try
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(ActionManager.mainConfig);

                File.WriteAllText(fileName, json);

                return true;
            }
            catch
            {
                return false;
            }

        }

        public static void ShowConfig()
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(ActionManager.mainConfig));
        }

        public static void SetConfigIpAddress(string ipAddress)
        {
            ActionManager.mainConfig.IPAddress = ipAddress;
        }

        public static void SetConfigPort(int port)
        {
            ActionManager.mainConfig.Port = port;
        }

        public static void SetConfigSecret(string secret)
        {
            ActionManager.mainConfig.Secret = secret;
        }

        public static void SetAccountGUID(string accountGUID)
        {
            ActionManager.mainConfig.AccountGUID = accountGUID;
        }

        public static bool CheckHaasConnection()
        {
            HaasonlineClient haasonlineClient = new HaasonlineClient(ActionManager.mainConfig.IPAddress, ActionManager.mainConfig.Port, ActionManager.mainConfig.Secret);

            var accounts = haasonlineClient.AccountDataApi.GetEnabledAccounts();

            if (accounts.Result.ErrorCode == Haasonline.LocalApi.CSharp.Enums.EnumErrorCode.Success)
            {
                return true;
            }

            return false;
        }

        public static List<Tuple<string,string>> GetAccountGUIDS()
        {

            List<Tuple<string, string>> results = new List<Tuple<string, string>>();

            if (ActionManager.CheckHaasConnection())
            {
                HaasonlineClient haasonlineClient = new HaasonlineClient(ActionManager.mainConfig.IPAddress, ActionManager.mainConfig.Port, ActionManager.mainConfig.Secret);

                var accounts = haasonlineClient.AccountDataApi.GetEnabledAccounts();

                // Quick hacky to get a key
                foreach (string x in accounts.Result.Result.Keys)
                {
                    results.Add(new Tuple<string, string>(accounts.Result.Result[x], x));
                }
            }

            return results;

        }

        public static List<string> GetMarkets()
        {

            List<string> results = new List<string>();

            if (ActionManager.CheckHaasConnection())
            {
                try
                {
                    HaasonlineClient haasonlineClient = new HaasonlineClient(ActionManager.mainConfig.IPAddress, ActionManager.mainConfig.Port, ActionManager.mainConfig.Secret);

                    AccountInformation accountInformation = haasonlineClient.AccountDataApi.GetAccountDetails(mainConfig.AccountGUID).Result.Result;

                    var markets = haasonlineClient.MarketDataApi.GetPriceMarkets(accountInformation.ConnectedPriceSource);

                    foreach (var market in markets.Result.Result)
                    {
                        if (market.SecondaryCurrency.Equals(mainConfig.PrimaryCurrency))
                        {
                            results.Add(market.PrimaryCurrency);
                        }
                    }
                }
                catch
                {
                    return results;
                }
            }

            return results;
        }

        public static void GrabMarketData(string market)
        {
            HaasonlineClient haasonlineClient = new HaasonlineClient(ActionManager.mainConfig.IPAddress, ActionManager.mainConfig.Port, ActionManager.mainConfig.Secret);

            AccountInformation accountInformation = haasonlineClient.AccountDataApi.GetAccountDetails(mainConfig.AccountGUID).Result.Result;

            var task = Task.Run(async () => await haasonlineClient.MarketDataApi.GetHistory(accountInformation.ConnectedPriceSource, market, mainConfig.PrimaryCurrency, "", 1, mainConfig.BackTestLength * 2));

            task.Wait();

        }

        public static bool PerformStartup()
        {
            if (LoadConfig())
            {
                return true;
            }
            else
            {
                ActionManager.mainConfig = new HaasConfig();
                return false;
            }
        }


    }
}
