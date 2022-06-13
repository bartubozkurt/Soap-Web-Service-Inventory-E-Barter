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

namespace ElectronicsInventory
{
    /// <summary>
    /// Çeşitli elektronik eşyaların fiyatlarını sağlar
    /// </summary>
    [WebService(Namespace = "http://localhost/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Bu Web Hizmeti'nin, ASP.NET AJAX kullanılarak komut dosyasından çağrılmasına, aşağıdaki satırı açıklamadan kaldırmasına olanak vermek için.
    [System.Web.Script.Services.ScriptService]
    public class ElectronicsInventory : System.Web.Services.WebService
    {

        string filePath = HttpRuntime.AppDomainAppPath + "dtElectronics.json";

       [WebMethod]
        public string ListAllElectronics() // tüm itemleri list eder.
        {
            string result = "";
            try
            {

                string jsonString = File.ReadAllText(filePath);
                ElectronicList electronics = JsonConvert.DeserializeObject<ElectronicList>(jsonString);
                foreach (Electronic e in electronics.Electronics)
                 {
                     result += e.ElectronicName + ";";
                 }
            }
            catch
            {
                return result;
            }
            return result;
        }
       
        [WebMethod]
        public bool AddElectronicPrice(String electronicName, double minElectronicPrice, double maxElectronicPrice) // Itemlerin fiyatlarını ekler
        {
            try
            {
                Electronic f = new Electronic
                {
                    ElectronicName = electronicName,
                    MinElectronicPrice = minElectronicPrice,
                    MaxElectronicPrice= maxElectronicPrice
                };
                string jsonString = File.ReadAllText(filePath);
                ElectronicList electronics = JsonConvert.DeserializeObject<ElectronicList>(jsonString);

                if (electronics == null) electronics = new ElectronicList();

                electronics.Electronics.Add(f);
                jsonString = JsonConvert.SerializeObject(electronics); 
                File.WriteAllText(filePath, jsonString);
            }
            catch
            {
                return false;
            }
            return true;
        }

        [WebMethod]
        public bool DeleteElectronicPrice(String electronicName) // Itemleri siler 
        {

            try
            {

                string jsonString = File.ReadAllText(filePath);
                ElectronicList electronics = JsonConvert.DeserializeObject<ElectronicList>(jsonString);
                int index = electronics.Electronics.FindIndex(item => item.ElectronicName == electronicName);
                if (index >= 0)
                {
                    electronics.Electronics.RemoveAt(index);
                    jsonString = JsonConvert.SerializeObject(electronics);
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
        public bool UpdateElectronicPrice(String electronicName, double EMinPrice, double EMaxPrice) // Itemlerin fiyatlarını günceller
        {
            try
            {

                string jsonString = File.ReadAllText(filePath);
                ElectronicList electronics = JsonConvert.DeserializeObject<ElectronicList>(jsonString);

                int index = electronics.Electronics.FindIndex(item => item.ElectronicName == electronicName);
                
                if (index >= 0)
                {
                    electronics.Electronics[index].MinElectronicPrice = EMinPrice; // min fiyat
                    electronics.Electronics[index].MaxElectronicPrice = EMaxPrice; // max fiyat
                    jsonString = JsonConvert.SerializeObject(electronics);
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
        public string CheckInventory(String electronicName)  // Item Inventoryde var mı ?
        {
            string result = "";
            try
            {

                string jsonString = File.ReadAllText(filePath);
                ElectronicList electronics = JsonConvert.DeserializeObject<ElectronicList>(jsonString);
                foreach (Electronic f in electronics.Electronics)
                {
                    if (f.ElectronicName.ToLower() == electronicName.ToLower())
                    {
                        result = f.ElectronicName;
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


        //min
        [WebMethod]
        public double CalMinElectronicCost(String electronicName, double electronicQnty) // min item tutarı için hesaplama yapar
        {
            double privMin = 0.0;
            try
            {

                string jsonString = File.ReadAllText(filePath);
                ElectronicList electronics = JsonConvert.DeserializeObject<ElectronicList>(jsonString);
                var elec = electronics.Electronics.Find(item => item.ElectronicName == electronicName);

                if (elec != null)
                {

                    privMin = elec.MinElectronicPrice * electronicQnty; // min fiyat

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
        public double CalMaxElectronicCost(String electronicName, double electronicQnty) // max item tutarı için hesaplama yapar
        {
            double privMax = 0.0;
            try
            {

                string jsonString = File.ReadAllText(filePath);
                ElectronicList electronics = JsonConvert.DeserializeObject<ElectronicList>(jsonString);
                var MInstrument = electronics.Electronics.Find(item => item.ElectronicName == electronicName);

                if (MInstrument != null)
                {  

                    privMax = MInstrument.MaxElectronicPrice * electronicQnty;  // max fiyat

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



        // minHowMuch
        [WebMethod]
        public double HowMuchMinInstrument(String elecName, double totalBudget) // total budget ile min item adedi hesaplar
        {
            double resultMin = 0.0;


            try
            {
                string jsonString = File.ReadAllText(filePath);
                ElectronicList electronics = JsonConvert.DeserializeObject<ElectronicList>(jsonString);
                var Elec = electronics.Electronics.Find(item => item.ElectronicName == elecName);

                if (Elec != null)
                {
                    resultMin = Math.Floor(totalBudget / Elec.MinElectronicPrice);

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
        }


        // maxHowMuch
        [WebMethod]
        public double HowMuchMaxInstrument(String elecName, double totalBudget) // total budget ile max item adedi hesaplar
        {
            double resultMax = 0.0;


            try
            {
                string jsonString = File.ReadAllText(filePath);
                ElectronicList electronics = JsonConvert.DeserializeObject<ElectronicList>(jsonString);
                var Ee = electronics.Electronics.Find(item => item.ElectronicName == elecName);

                if (Ee != null)
                {
                    resultMax = Math.Floor(totalBudget / Ee.MaxElectronicPrice);

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
        }


    }
    public class Electronic
    {
        public string ElectronicName { get; set; }
        public double ElectronicPrice { get; set; }

        public double MinElectronicPrice { get; set; }
        public double MaxElectronicPrice { get; set; }
    }

    public class ElectronicList
    {
        public ElectronicList()
        {
            Electronics = new List<Electronic>();
        }

        public List<Electronic> Electronics { get; set; }
    }
}
