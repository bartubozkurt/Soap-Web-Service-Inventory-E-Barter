using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using System.IO;
using Microsoft.SqlServer.Server;

namespace DrinkInventory
{
    /// <summary>
    /// Summary description for DrinkInventory
    /// </summary>
    [WebService(Namespace = "https://localhost:44330/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class DrinkInventory : System.Web.Services.WebService
    {

        string filePath = HttpRuntime.AppDomainAppPath + "dtDrink.json";

        // ListAllDrinks

        [WebMethod]
        public string ListAllDrinks() // Inventorydeki tüm itemleri listeler
        {
            string result = "";
            try
            {
                string jsonString = File.ReadAllText(filePath);
                DrinkList drinks = JsonConvert.DeserializeObject<DrinkList>(jsonString);
                foreach (Drink m in drinks.Drinks_List)
                {
                    result += m.DrinkName + ";";
                }
                jsonString = JsonConvert.SerializeObject(drinks);
                File.WriteAllText(filePath, jsonString);
            }
            catch
            {
                return result;
            }
            return result;
        }

        // AddDrinkPrice

        [WebMethod]
        public bool AddDrinkPrice(String drinkName, double minDrinkPrice, double maxDrinkPrice, Unit drinkUnit) // İnventorye yeni item ekler 
        {
            try
            {
                Drink d = new Drink
                {
                    DrinkName = drinkName,
                    DrinkUnit = drinkUnit,
                    MinDrinkPrice = minDrinkPrice,
                    MaxDrinkPrice = maxDrinkPrice
                };

                string jsonString = File.ReadAllText(filePath);
                DrinkList drinks = JsonConvert.DeserializeObject<DrinkList>(jsonString);

                if (drinks == null) drinks = new DrinkList();

                drinks.Drinks_List.Add(d);
                jsonString = JsonConvert.SerializeObject(drinks);
                File.WriteAllText(filePath, jsonString);
            }
            catch
            {
                return false;
            }
            return true;
        }


        //DeleteDrinkPrice

        [WebMethod]
        public bool DeleteDrinkPrice(String drinkName) // Inventorydeki itemlerin fiyatlarını siler
        {

            try
            {

                string jsonString = File.ReadAllText(filePath);
                DrinkList drinks = JsonConvert.DeserializeObject<DrinkList>(jsonString);
                int index = drinks.Drinks_List.FindIndex(item => item.DrinkName == drinkName);
                if (index >= 0)
                {
                    drinks.Drinks_List.RemoveAt(index);
                    jsonString = JsonConvert.SerializeObject(drinks);
                    File.WriteAllText(filePath, jsonString);
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
            return true;
        }


        //UpdateDrinkPrice

        [WebMethod]
        public bool UpdateDrinkPrice(String drinkName, double minDrinkPrice, double maxDrinkPrice) // Invetorydeki halihazırdaki itemlerin fiyatlarını update eder.
        {
            try
            {

                string jsonString = File.ReadAllText(filePath);
                DrinkList drinks = JsonConvert.DeserializeObject<DrinkList>(jsonString);
                int index = drinks.Drinks_List.FindIndex(item => item.DrinkName == drinkName);
                if (index >= 0)
                {
                    drinks.Drinks_List[index].MinDrinkPrice = minDrinkPrice;  // min price update
                    drinks.Drinks_List[index].MaxDrinkPrice = maxDrinkPrice; // max price update
                    jsonString = JsonConvert.SerializeObject(drinks);
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
        public double CalMinDrinkCost(String drinkName, double drinkQuantity)  // Item'in Min cost'unu hesaplar
        {
            double privMin = 0.0;
            try
            {

                string jsonString = File.ReadAllText(filePath);
                DrinkList drinks = JsonConvert.DeserializeObject<DrinkList>(jsonString);
                var ddrinks = drinks.Drinks_List.Find(item => item.DrinkName == drinkName);

                if (ddrinks != null)
                {

                    privMin = ddrinks.MinDrinkPrice * drinkQuantity;  // min drink price * Quantity

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

        //max
        [WebMethod]
        public double CalMaxDrinkCost(String drinkName, double drinkQuantity) // Item'in Max cost'unu hesaplar
        {
            double privMax = 0.0;
            try
            {

                string jsonString = File.ReadAllText(filePath);
                DrinkList drinks = JsonConvert.DeserializeObject<DrinkList>(jsonString);
                var ddrinks = drinks.Drinks_List.Find(item => item.DrinkName == drinkName);

                if (ddrinks != null)
                {

                    privMax = ddrinks.MaxDrinkPrice * drinkQuantity; // Max0 drink price * Quantity

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

        // CheckInventory

        [WebMethod]
        public string CheckInventory(String drinkName)
        {
            string result = "";
            try
            {

                string jsonString = File.ReadAllText(filePath);
                DrinkList drinks = JsonConvert.DeserializeObject<DrinkList>(jsonString);
                foreach (Drink d in drinks.Drinks_List)
                {
                    if (d.DrinkName.ToLower() == drinkName.ToLower())
                    {
                        result = d.DrinkName;
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
        public double HowMuchMinInstrument(String drinkName, double totalBudget)  // verilen total budget'ta ne kadar min item alabilir onu hesaplar
        {
            double resultMin = 0.0;

            try
            {
                string jsonString = File.ReadAllText(filePath);
                DrinkList drinks = JsonConvert.DeserializeObject<DrinkList>(jsonString);

                var ddrinks = drinks.Drinks_List.Find(item => item.DrinkName == drinkName);

                if (ddrinks != null)
                {
                    if (ddrinks.DrinkUnit == Unit.LT)  // eğer unit LT ise result min double hesaplar (örn. 1.1)
                    {
                        resultMin = totalBudget / ddrinks.MinDrinkPrice;
                    }

                    else
                    {
                        resultMin = Math.Floor(totalBudget / ddrinks.MinDrinkPrice); // değilse aşağı yuvarlayarak min hesaplar  ( 1.1 ---> 1 )
                        return resultMin;
                    }
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
            return resultMin;
        }

        // maxHowMuch
        [WebMethod]
        public double HowMuchMaxInstrument(String drinkName, double totalBudget)  // verilen total budget'ta ne kadar max item alabilir onu hesaplar
        {
            double resultMax = 0.0;

            try
            {
                string jsonString = File.ReadAllText(filePath);
                DrinkList drinks = JsonConvert.DeserializeObject<DrinkList>(jsonString);
                var ddrinks = drinks.Drinks_List.Find(item => item.DrinkName == drinkName);

                if (ddrinks != null)
                {
                    if (ddrinks.DrinkUnit == Unit.LT)
                    {
                        resultMax = totalBudget / ddrinks.MaxDrinkPrice; // eğer unit LT ise result Max item  double hesaplar (örn. 1.1)
                    }

                    else
                    {
                        resultMax = Math.Floor(totalBudget / ddrinks.MaxDrinkPrice); // değilse aşağı yuvarlayarak max item hesaplar  ( 1.1 ---> 1 )
                        return resultMax;
                    }
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
            return resultMax;
        }

        public enum Unit
        {
            LT,
            Adet
        }

        public class Drink
        {
            public string DrinkName { get; set; }

            public double MinDrinkPrice { get; set; }
            public double MaxDrinkPrice { get; set; }

            public Unit DrinkUnit { get; set; }
        }

        public class DrinkList
        {

            public DrinkList()
            {
                Drinks_List = new List<Drink>();
            }
            public List<Drink> Drinks_List { get; set; }
        }
    }
}
