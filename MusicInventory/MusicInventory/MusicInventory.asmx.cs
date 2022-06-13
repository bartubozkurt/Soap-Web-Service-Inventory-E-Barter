using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using System.Web;
using System.Web.Services;
using System.Text.Json;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using System.IO;
using Microsoft.SqlServer.Server;


namespace MusicInventory
{
    /// <summary>
    /// Summary description for MusicInventory
    /// </summary>
    [WebService(Namespace = "https://localhost:44330/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class MusicInventory : System.Web.Services.WebService
    {
        string filePath = HttpRuntime.AppDomainAppPath + "dtMusic.json";
        


        //! ListAllInstruments

        [WebMethod]
        public string ListAllInstruments()  // itemleri listeler 
        {
            string result = "";
            try
            {
                string jsonString = File.ReadAllText(filePath);
                MusicInstrumentsList Instruments = JsonConvert.DeserializeObject<MusicInstrumentsList>(jsonString);
                foreach(MusicInstruments m in Instruments.MusicInstruments_List)
                {
                    result += m.InstrumentsName + ";";
                }
                jsonString = JsonConvert.SerializeObject(Instruments);  
                File.WriteAllText(filePath,jsonString);
            }
            catch
            {
                return result;
            }
            return result;
        }

        [WebMethod]
        public bool AddInstrument(String instrumentName, double minPrice, double  maxPrice) // itemlerin fiyatlarını Inventory ye ekler.
        {
            try
            {
                MusicInstruments m = new MusicInstruments
                {
                    InstrumentsName = instrumentName,
                    MinInstrumentPrice = minPrice,
                    MaxInstrumentPrice = maxPrice
                    
                };
                
                string jsonString = File.ReadAllText(filePath);
                MusicInstrumentsList Mlist = JsonConvert.DeserializeObject<MusicInstrumentsList>(jsonString);

                if (Mlist == null) Mlist = new MusicInstrumentsList();
                
                Mlist.MusicInstruments_List.Add(m);
                jsonString = JsonConvert.SerializeObject(Mlist);
                File.WriteAllText(filePath, jsonString);
            }
            catch
            {
                return false;
            }
            return true;
        }

        //! DeleteInstrumentPrice

        [WebMethod]
        public bool DeleteInstrumentPrice(String instrumentName)  // Itemlerin Fiyatlarını Siler
        {
            try
            {
                string jsonString = File.ReadAllText(filePath);
                MusicInstrumentsList Mlist = JsonConvert.DeserializeObject<MusicInstrumentsList>(jsonString);
                int index = Mlist.MusicInstruments_List.FindIndex(item => item.InstrumentsName == instrumentName);
                if(index >= 0)
                {
                    Mlist.MusicInstruments_List.RemoveAt(index);
                    jsonString = JsonConvert.SerializeObject(Mlist);
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

        //! UpdateInstrumentPrice

        [WebMethod]
        public bool UpdateInstrumentPrice(String instrumentName, double instrumentMinPrice, double instrumentMaxPrice) // Itemlerin fiyatlarını Update eder
        {
            try
            {
                string jsonString = File.ReadAllText(filePath);
                MusicInstrumentsList Mlist = JsonConvert.DeserializeObject<MusicInstrumentsList>(jsonString);
                int index = Mlist.MusicInstruments_List.FindIndex(item => item.InstrumentsName == instrumentName);
                if (index >= 0)
                {
                    Mlist.MusicInstruments_List[index].MinInstrumentPrice = instrumentMinPrice;
                    Mlist.MusicInstruments_List[index].MaxInstrumentPrice = instrumentMaxPrice;
                    jsonString = JsonConvert.SerializeObject(Mlist);
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
        public double CalMinInstrumentCost(String instrumentName, double instrumentQuantity) // Itemlerin Min  fiyatlarını hesaplar
        {
            double privMin = 0.0;
            try
            {

                string jsonString = File.ReadAllText(filePath);
                MusicInstrumentsList Mlist = JsonConvert.DeserializeObject<MusicInstrumentsList>(jsonString);
                var MInstrument = Mlist.MusicInstruments_List.Find(item => item.InstrumentsName == instrumentName);

                if (MInstrument != null)
                {

                    privMin = MInstrument.MinInstrumentPrice * instrumentQuantity;

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
        public double CalMaxInstrumentCost(String instrumentName, double instrumentQuantity) // Itemlerin Max fiyatlarını hesaplar
        {
            double privMax = 0.0;
            try
            {

                string jsonString = File.ReadAllText(filePath);
                MusicInstrumentsList Mlist = JsonConvert.DeserializeObject<MusicInstrumentsList>(jsonString);
                var MInstrument = Mlist.MusicInstruments_List.Find(item => item.InstrumentsName == instrumentName);

                if (MInstrument != null)
                {

                    privMax = MInstrument.MaxInstrumentPrice * instrumentQuantity;

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
        public string CheckInventory(String instrumentName) // Item Inventory de var mı onu kontrol eder
        {
            string result = "";
            try
            {
                string jsonString = File.ReadAllText(filePath);
                MusicInstrumentsList Mlist = JsonConvert.DeserializeObject<MusicInstrumentsList>(jsonString);
                foreach (MusicInstruments m in Mlist.MusicInstruments_List)
                {
                    if(m.InstrumentsName.ToLower() == instrumentName.ToLower())
                    {
                        result = m.InstrumentsName;
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
        public double HowMuchMinInstrument(String instrumentName, double totalBudget) // Total budget ile alınabilecek min itemi hesaplar
        {
            double resultMin = 0.0;


            try
            {
                string jsonString = File.ReadAllText(filePath);
                MusicInstrumentsList Mlist = JsonConvert.DeserializeObject<MusicInstrumentsList>(jsonString);
                var MInstrument = Mlist.MusicInstruments_List.Find(item => item.InstrumentsName == instrumentName);

                if (MInstrument != null)
                {
                    resultMin = Math.Floor(totalBudget / MInstrument.MinInstrumentPrice);  

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
        public double HowMuchMaxInstrument(String instrumentName, double totalBudget) // Total budget ile alınabilecek max itemi hesaplar
        {
            double resultMax = 0.0;


            try
            {
                string jsonString = File.ReadAllText(filePath);
                MusicInstrumentsList Mlist = JsonConvert.DeserializeObject<MusicInstrumentsList>(jsonString);
                var MInstrument = Mlist.MusicInstruments_List.Find(item => item.InstrumentsName == instrumentName);

                if (MInstrument != null)
                {
                    resultMax = Math.Floor(totalBudget / MInstrument.MaxInstrumentPrice);

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


        public class MusicInstruments
        {
            public string InstrumentsName { get; set; }

            public double MinInstrumentPrice { get; set; }
            public double MaxInstrumentPrice { get; set; }   


        }

        public class MusicInstrumentsList
        {

            public MusicInstrumentsList()
            {
                MusicInstruments_List = new List<MusicInstruments>();
            }

            public List<MusicInstruments> MusicInstruments_List { get; set; }
        }


    }
}
