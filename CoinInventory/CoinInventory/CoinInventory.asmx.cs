using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json;
using System.IO;
using Microsoft.SqlServer.Server;

namespace CoinInventory
{
    /// <summary>
    /// Summary description for CoinInventory
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class CoinInventory : System.Web.Services.WebService
    {
        string filePath = HttpRuntime.AppDomainAppPath + "dtCoin.json";       


    [WebMethod]
    public string ListAllCoins()  // itemleri listeler 
    {
        string result = "";
        try
        {
            string jsonString = File.ReadAllText(filePath);
            CoinsList Coin = JsonConvert.DeserializeObject<CoinsList>(jsonString);
            foreach (Coins m in Coin.Coins_List)
            {
                result += m.CoinName + ";";
            }
            jsonString = JsonConvert.SerializeObject(Coin);
            File.WriteAllText(filePath, jsonString);
        }
        catch
        {
            return result;
        }
        return result;
    }

    [WebMethod]
    public bool AddCoinsPrice(String coinName, double minPrice, double maxPrice) // itemlerin fiyatlarını Inventory ye ekler.
    {
        try
        {
            Coins m = new Coins
            {
                CoinName = coinName,
                MinCoinPrice = minPrice,
                MaxCoinPrice = maxPrice

            };

            string jsonString = File.ReadAllText(filePath);
            CoinsList cList = JsonConvert.DeserializeObject<CoinsList>(jsonString);

            if (cList == null) cList = new CoinsList();

            cList.Coins_List.Add(m);
            jsonString = JsonConvert.SerializeObject(cList);
            File.WriteAllText(filePath, jsonString);
        }
        catch
        {
            return false;
        }
        return true;
    }

    //! DeleteCoinsPrice

    [WebMethod]
    public bool DeleteCoinsPrice(String coinName)  // Itemlerin Fiyatlarını Siler
    {
        try
        {
            string jsonString = File.ReadAllText(filePath);
            CoinsList cList = JsonConvert.DeserializeObject<CoinsList>(jsonString);
            int index = cList.Coins_List.FindIndex(item => item.CoinName == coinName);
            if (index >= 0)
            {
                cList.Coins_List.RemoveAt(index);
                jsonString = JsonConvert.SerializeObject(cList);
                File.WriteAllText(filePath, jsonString);
            }
            else
            {
                return false;
            }
        }
        catch
        {
            return false;
        }
        return true;
    }


    //! UpdateCoinsPrice

    [WebMethod]
    public bool UpdateCoinsPrice(String coinName, double coinMinPrice, double coinMaxPrice) // Itemlerin fiyatlarını Update eder
    {
        try
        {
            string jsonString = File.ReadAllText(filePath);
            CoinsList cList = JsonConvert.DeserializeObject<CoinsList>(jsonString);
            int index = cList.Coins_List.FindIndex(item => item.CoinName == coinName);
            if (index >= 0)
            {
                cList.Coins_List[index].MinCoinPrice = coinMinPrice;
                cList.Coins_List[index].MaxCoinPrice = coinMaxPrice;
                jsonString = JsonConvert.SerializeObject(cList);
                File.WriteAllText(filePath, jsonString);
            }
            else
            {
                return false;
            }
        }
        catch
        {
            return false;
        }
        return true;
    }


    //min
    [WebMethod]
    public double CalMinCoinsCost(String coinName, double coinQuantity) // Itemlerin Min  fiyatlarını hesaplar
    {
        double privMin = 0.0;
        try
        {

            string jsonString = File.ReadAllText(filePath);
            CoinsList cList = JsonConvert.DeserializeObject<CoinsList>(jsonString);
            var Coi = cList.Coins_List.Find(item => item.CoinName == coinName);

            if (Coi != null)
            {

                privMin = Coi.MinCoinPrice * coinQuantity;

                return privMin;
            }
            else
            {
                throw new InvalidOperationException("Invalid Operation");
            }
        }
        catch
        {
            throw new InvalidOperationException("Invalid Operation");
        }
    }

    //Max
    [WebMethod]
    public double CalMaxCoinsCost(String coinName, double coinQuantity) // Itemlerin Max fiyatlarını hesaplar
    {
        double privMax = 0.0;
        try
        {

            string jsonString = File.ReadAllText(filePath);
            CoinsList cList = JsonConvert.DeserializeObject<CoinsList>(jsonString);
            var Coi = cList.Coins_List.Find(item => item.CoinName == coinName);

            if (Coi != null)
            {

                privMax = Coi.MaxCoinPrice * coinQuantity;

                return privMax;
            }
            else
            {
                throw new InvalidOperationException("Invalid Operation");
            }
        }
        catch
        {
            throw new InvalidOperationException("Invalid Operation");
        }
    }


    //! CheckInventory

    [WebMethod]
    public string CheckInventory(String coinName) // Item Inventory de var mı onu kontrol eder
    {
        string result = "";
        try
        {
            string jsonString = File.ReadAllText(filePath);
            CoinsList cList = JsonConvert.DeserializeObject<CoinsList>(jsonString);
            foreach (Coins m in cList.Coins_List)
            {
                if (m.CoinName.ToLower() == coinName.ToLower())
                {
                    result = m.CoinName;
                    break;
                }
            }
        }
        catch
        {
            return result;
        }
        return result;
    }

    // minHowMuch
    [WebMethod]
    public double HowMuchMinCoins(String coinName, double totalBudget) // Total budget ile alınabilecek min itemi hesaplar
    {
        double resultMin = 0.0;


        try
        {
            string jsonString = File.ReadAllText(filePath);
            CoinsList cList = JsonConvert.DeserializeObject<CoinsList>(jsonString);
            var Coi = cList.Coins_List.Find(item => item.CoinName == coinName);

            if (Coi != null)
            {
                resultMin = totalBudget / Coi.MinCoinPrice;

                return resultMin;
            }
            else
            {
                return resultMin;
            }
        }
        catch
        {
            return resultMin;
        }

        //return resultMin;
    }

    // MaxHowMuch
    [WebMethod]
    public double HowMuchMaxCoins(String coinName, double totalBudget) // Total budget ile alınabilecek max itemi hesaplar
    {
        double resultMax = 0.0;


        try
        {
            string jsonString = File.ReadAllText(filePath);
            CoinsList cList = JsonConvert.DeserializeObject<CoinsList>(jsonString);
            var Coi = cList.Coins_List.Find(item => item.CoinName == coinName);

            if (Coi != null)
            {
                resultMax = totalBudget / Coi.MaxCoinPrice;

                return resultMax;
            }
            else
            {
                return resultMax;
            }
        }
        catch
        {
            return resultMax;
        }

        //return resultMax;
    }


    }

    public class Coins
    {
        public string CoinName { get; set; }

        public double MinCoinPrice { get; set; }
        public double MaxCoinPrice { get; set; }
    }

    public class CoinsList
    {

        public CoinsList()
        {
            Coins_List = new List<Coins>();
        }

        public List<Coins> Coins_List { get; set; }
    }

}
