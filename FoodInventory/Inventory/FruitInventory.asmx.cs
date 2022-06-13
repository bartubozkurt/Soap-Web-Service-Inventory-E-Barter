using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
namespace Inventory
{
    /// <summary>
    /// Çeşitli meyvelerin fiyatlarını sağlar.
    /// </summary>
    [WebService(Namespace = "http://localhost/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class FruitInventory : System.Web.Services.WebService
    {
        
        string filePath = HttpRuntime.AppDomainAppPath + "dtFurit.json";

         [WebMethod]
        public string ListAllFruits() // tum itemleri listeler
        {
            string result = "";
            try
            {
               
                string jsonString = File.ReadAllText(filePath);
                FruitList fruits = JsonConvert.DeserializeObject<FruitList>(jsonString);
                foreach (Fruit f in fruits.Fruits)
                {
                    result += f.FruitName + ";";
                }
                jsonString = JsonConvert.SerializeObject(fruits);
                File.WriteAllText(filePath, jsonString);
            }
            catch
            {
                return result;
            }
            return result;
        }

        [WebMethod]
        public bool AddFruitPrice(String fruitName, double minfruitPrice, Unit fruitUnit, double maxfruitPrice) // itemleri Inventory ye ekler
        {
            try
            {
                Fruit f = new Fruit
                {
                    FruitName = fruitName,
                    MaxFruitPrice = maxfruitPrice,
                    MinFruitPrice = minfruitPrice,

                    FruitUnit = fruitUnit
                };

                string jsonString = File.ReadAllText(filePath);
                FruitList fruits = JsonConvert.DeserializeObject<FruitList>(jsonString);

                if (fruits == null) fruits = new FruitList();

                fruits.Fruits.Add(f);
                jsonString = JsonConvert.SerializeObject(fruits);
                File.WriteAllText(filePath, jsonString);
            }
            catch
            {
                return false;
            }
            return true;
        }

        [WebMethod]
        public bool DeleteFruitPrice(String fruitName) // Itemleri Inventoryden siler
        {

            try
            {
                
                string jsonString = File.ReadAllText(filePath);
                FruitList fruits = JsonConvert.DeserializeObject<FruitList>(jsonString);
                int index=fruits.Fruits.FindIndex(item => item.FruitName == fruitName);
                if (index >= 0)
                {
                    fruits.Fruits.RemoveAt(index);
                    jsonString = JsonConvert.SerializeObject(fruits);
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


        [WebMethod]
        public bool UpdateFruitPrice(String fruitName, double minFruitPrice, double maxFruitPrice) // Itemlerin fiyatlarını update eder
        {
            try
            {

                string jsonString = File.ReadAllText(filePath);
                FruitList fruits = JsonConvert.DeserializeObject<FruitList>(jsonString);
                int index = fruits.Fruits.FindIndex(item => item.FruitName == fruitName);
                if (index >= 0)
                {
                    fruits.Fruits[index].MinFruitPrice = minFruitPrice; // min fiyat 
                    fruits.Fruits[index].MaxFruitPrice = maxFruitPrice; // max fiyat
                    jsonString = JsonConvert.SerializeObject(fruits);
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
        public double CalMinFruitCost(String fruitName, double fruitQuantity) // Min item fiyatı hesabı yapar
        {
            double privMin = 0.0;
            try
            {

                string jsonString = File.ReadAllText(filePath);
                FruitList fruits = JsonConvert.DeserializeObject<FruitList>(jsonString);
                var fr = fruits.Fruits.Find(item => item.FruitName == fruitName);

                if (fr != null)
                {

                    privMin = fr.MinFruitPrice * fruitQuantity;

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
        public double CalMaxFruitCost(String fruitName, double fruitQuantity) // Max item fiyatı hesabı yapar
        {
            double privMax = 0.0;
            try
            {

                string jsonString = File.ReadAllText(filePath);
                FruitList fruits = JsonConvert.DeserializeObject<FruitList>(jsonString);
                var fr = fruits.Fruits.Find(item => item.FruitName == fruitName);

                if (fr != null)
                {

                    privMax = fr.MaxFruitPrice * fruitQuantity;

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


        [WebMethod]
        public string CheckInventory(String fruitName) // item Inventory de var mı ?
        {
            string result = "";
            try
            {

                string jsonString = File.ReadAllText(filePath);
                FruitList fruits = JsonConvert.DeserializeObject<FruitList>(jsonString);
                foreach (Fruit f in fruits.Fruits)
                {
                    if(f.FruitName.ToLower() == fruitName.ToLower())
                    {
                        result = f.FruitName;
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
        public double HowMuchMinFruit(String fruitName, double totalBudget) // total budget la alınabilen min item adedi 
        {
            double resultMin = 0.0;

            try
            {
                string jsonString = File.ReadAllText(filePath);
                FruitList fruits = JsonConvert.DeserializeObject<FruitList>(jsonString);

                var fr = fruits.Fruits.Find(item => item.FruitName == fruitName);

                if (fr != null)
                {
                    if (fr.FruitUnit == Unit.KG)
                    {
                        resultMin = totalBudget / fr.MinFruitPrice;
                    }

                    else
                    {
                        resultMin = Math.Floor(totalBudget / fr.MinFruitPrice);
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
        public double HowMuchMaxFruit(String fruitName, double totalBudget)  // total budget la alınabilen max item adedi 
        {
            double resultMax = 0.0;

            try
            {
                string jsonString = File.ReadAllText(filePath);
                FruitList fruits = JsonConvert.DeserializeObject<FruitList>(jsonString);

                var fr = fruits.Fruits.Find(item => item.FruitName == fruitName);

                if (fr != null)
                {
                    if (fr.FruitUnit == Unit.KG)
                    {
                        resultMax = totalBudget / fr.MaxFruitPrice;
                    }

                    else
                    {
                        resultMax = Math.Floor(totalBudget / fr.MaxFruitPrice);
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

    }

    public enum Unit
    {
        KG,
        Adet
    }
    
    public class Fruit
    {
        public string FruitName { get; set; }

        public double MinFruitPrice { get; set; }
        public double MaxFruitPrice { get; set; }

        public Unit FruitUnit { get; set; }
    }

    public class FruitList
    {
        public FruitList()
        {
            Fruits = new List<Fruit>();
        }

        public List<Fruit> Fruits { get; set; }
    }
}
