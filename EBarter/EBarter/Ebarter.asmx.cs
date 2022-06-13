using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Services;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Runtime.Remoting.Messaging;

namespace EBarter
{
    [WebService(Namespace = "http://localhost/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Bu Web Hizmeti'nin, ASP.NET AJAX kullanılarak komut dosyasından çağrılmasına, aşağıdaki satırı açıklamadan kaldırmasına olanak vermek için.
    [System.Web.Script.Services.ScriptService]
    public class Ebarter : System.Web.Services.WebService
    {
        string filePath_1 = HttpRuntime.AppDomainAppPath + "dtAcounts.json";


        // ======================================= Hesap Oluşturma ======================================= //

        [WebMethod]
        public string CreateAcount(string rName)
        {
            try
            {

                SHA1 sha = new SHA1CryptoServiceProvider();
                string shaName = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(rName)));

                Acount ac = new Acount()
                {
                    Name = rName,
                    Key = shaName
                };

                string jsonString = File.ReadAllText(filePath_1);

                AcountList acts = JsonConvert.DeserializeObject<AcountList>(jsonString);


                if (acts == null) acts = new AcountList();


                if (acts.Acounts.FindIndex(x => x.Name == rName) > -1)
                {
                    return "Aynı isimli bir kullanıcı var!";
                }
                else
                {
                    acts.Acounts.Add(ac);
                    jsonString = JsonConvert.SerializeObject(acts, Formatting.Indented);
                    File.WriteAllText(filePath_1, jsonString);
                    return shaName;
                }
            }
            catch
            {
                return "Kullanıcı oluşturulamadı!";
            }
            //return "";
        }

        // ======================================= Takas Hesaplama ======================================= //

        [WebMethod]
        public double BarterCalculater(String acName, string acKey, string requestedGood, string givenGood, double gN,double requestNumber)
        {

            double budgetMinGiven = 0.0;
            double budgetMaxGiven = 0.0;

            double budgetMinRequest = 0.0;
            double budgetMaxRequest = 0.0;

            try
            {
                string jsonString = File.ReadAllText(filePath_1);
                AcountList acts = JsonConvert.DeserializeObject<AcountList>(jsonString);

                int n = acts.Acounts.FindIndex(x => x.Name == acName);

                if (n > -1)
                {
                    if (acts.Acounts[n].Key != acKey)
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }


                // ======================================= Servisler ======================================= //

                FruitService.FruitInventorySoapClient fC = new FruitService.FruitInventorySoapClient();  // # Fruit # //

                ElectronicService.ElectronicsInventorySoapClient eC = new ElectronicService.ElectronicsInventorySoapClient(); // # Electronic # //

                MusicInventory.MusicInventorySoapClient mC = new MusicInventory.MusicInventorySoapClient();  // # Music # //

                DrinkInventory.DrinkInventorySoapClient dC = new DrinkInventory.DrinkInventorySoapClient();  // # Drink # //
                CoinInventoryService.CoinInventorySoapClient cS = new CoinInventoryService.CoinInventorySoapClient(); // # Coins # //


                string music;
                string drink;
                string electronic;
                string fruit;
                string coins;


                string musicR;
                string drinkR;
                string electronicR;
                string fruitR;
                string coinsR;


                // ======================================= Given good ======================================= //

                if ((music = mC.CheckInventory(givenGood)) != "")  // # Music # //
                {
                    budgetMinGiven = (mC.CalMinInstrumentCost(music,(int)gN));
                    budgetMaxGiven = (mC.CalMaxInstrumentCost(music,(int)gN));
                }

                else if ((drink = dC.CheckInventory(givenGood)) != "") // # Drink # //
                {
                    budgetMinGiven = (dC.CalMinDrinkCost(drink, gN));
                    budgetMaxGiven = (dC.CalMaxDrinkCost(drink, gN));
                }

                else if ((electronic = eC.CheckInventory(givenGood)) != "")  // # Electronic # //
                {
                    budgetMinGiven = (eC.CalMinElectronicCost(electronic, (int)gN));
                    budgetMaxGiven = (eC.CalMaxElectronicCost(electronic, (int)gN));
                }

                else if ((fruit = fC.CheckInventory(givenGood)) != "")  // # Fruit # //
                {
                    budgetMinGiven = (fC.CalMinFruitCost(fruit, (int)gN));
                    budgetMaxGiven = (fC.CalMaxFruitCost(fruit, (int)gN));
                }
                else if ((coins = cS.CheckInventory(givenGood)) != "") // # Coins # //
                {
                    budgetMinGiven = (cS.CalMinCoinsCost(coins, gN));
                    budgetMaxGiven = (cS.CalMaxCoinsCost(coins, gN));
                }


                // ======================================= Request good ======================================= //

                if (budgetMinGiven > 0.0 && budgetMaxGiven > 0.0)
                {
                    if((musicR = mC.CheckInventory(requestedGood)) != "")                                 // # Music # //
                    {
                        budgetMinRequest = (mC.CalMinInstrumentCost(musicR, requestNumber));
                        budgetMaxRequest = (mC.CalMaxInstrumentCost(musicR, requestNumber));
                    }
                    else if ((drinkR = dC.CheckInventory(requestedGood)) != "")                          // # Drink # //
                    {
                        budgetMinRequest = (dC.CalMinDrinkCost(drinkR, requestNumber));
                        budgetMaxRequest = (dC.CalMaxDrinkCost(drinkR, requestNumber));
                    }

                    else if ((electronicR = eC.CheckInventory(requestedGood)) != "")                    // # Electronic # //  
                    {
                        budgetMinRequest = (eC.CalMinElectronicCost(electronicR, requestNumber));
                        budgetMaxRequest = (eC.CalMaxElectronicCost(electronicR, requestNumber));
                    }
                    else if ((fruitR = fC.CheckInventory(requestedGood)) != "")                         // # Fruit # // 
                    {
                        budgetMinRequest = (fC.CalMinFruitCost(fruitR, requestNumber));
                        budgetMaxRequest = (fC.CalMaxFruitCost(fruitR, requestNumber));
                    }
                    else if ((coinsR = cS.CheckInventory(requestedGood)) != "")                         // # Coins # // 
                    {
                        budgetMinRequest = (cS.CalMinCoinsCost(coinsR, requestNumber));
                        budgetMaxRequest = (cS.CalMaxCoinsCost(coinsR, requestNumber));
                    }
                }

                // ======================================= Aralık hesaplama Çakışma kontrolü ======================================= //

                if ((budgetMinGiven >= budgetMinRequest && budgetMinGiven <= budgetMaxRequest) || (budgetMaxGiven >= budgetMinRequest && budgetMaxGiven <= budgetMaxRequest))
                {
                    return 1;  // Takas Gerçekleşti !
                }
                else
                {
                    return 0; // Takas Gerçekleşmedi !
                }

            }
            catch
            {
                return -1; //  Hata !
            }

        }
        //public enum Currency { TRY, BRL, UHT }

        //[WebMethod]
        //public double ConversionRate(Currency from, Currency to)
        //{
        //    return 0;
        //}

        public class Acount
        {
            public string Name { get; set; }
            public string Key { get; set; }
        }

        public class AcountList
        {
            public AcountList()
            {
                Acounts = new List<Acount>();
            }
            public List<Acount> Acounts { get; set; }
        }
    }

}
