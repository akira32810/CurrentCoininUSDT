
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Media;
namespace CryptoAlertAmount
{
    public  class Program
    {

        public static async Task Main(string[] args)
        {
            try
            {

                string[] currencydata = getCurrencyAndValueForAlert();
                string value = string.Empty;

                Console.WriteLine("Data as follow: ");
                Console.WriteLine("The current Currency to get: {0}", currencydata[0]);
                Console.WriteLine("The currenncy for the value to go above is set to: {0}", currencydata[1]);
                Console.WriteLine("The currency for the value to go below is set to: {0}", currencydata[2]);


                Console.WriteLine("---------------------\\n");

                while (true)
                {

                    Console.WriteLine("Do you want to get the value for above or below for the currency?(a for above, b for below)");
                    value = Console.ReadLine();
                    if (value == "a" || value == "b")
                    {
                        break;
                    }
                }




                if (value.ToLower() == "a")
                {
                    while (!Console.KeyAvailable || Console.ReadKey(true).Key != ConsoleKey.Enter)
                    {
                        decimal coinValue = await getCoinValue();

                        Console.WriteLine("the current value of {0} is {1} USDT", currencydata[0], coinValue.ToString());

                        if (coinValue > decimal.Parse(currencydata[1]))
                        {

                            Console.WriteLine("coin value is above your value you want of {0} ", currencydata[1]);
                            SoundPlayer player = new SoundPlayer(currencydata[4]);
                            player.Play();
                        }

                        Thread.Sleep(int.Parse(currencydata[3]) * 1000); // Wait for one minute
                    }


                    //value for above
                }

                if (value.ToLower() == "b")
                {
                    while (!Console.KeyAvailable || Console.ReadKey(true).Key != ConsoleKey.Enter)
                    {
                        decimal coinValue = await getCoinValue();

                        Console.WriteLine("the current value of {0} is {1} USDT", currencydata[0], coinValue.ToString());

                        if (coinValue < decimal.Parse(currencydata[2]))
                        {

                            Console.WriteLine("coin value is below your value you want of {0} ", currencydata[1]);
                            SoundPlayer player = new SoundPlayer(currencydata[4]);
                            player.Play();

                        }

                        Thread.Sleep(int.Parse(currencydata[3]) * 1000); // Wait for one minute
                    }

                }
            }

            catch (Exception e) 
            
            {
                Console.WriteLine("Error: {0}", e.Message);
                    
            }


        }


        public static string[] getCurrencyAndValueForAlert()
        {
          

            string[] dataValue = new string[5];

            dataValue[0] = ConfigurationManager.AppSettings["currency"].ToString();
            dataValue[1] = ConfigurationManager.AppSettings["curValtoGoAbove"].ToString();
            dataValue[2] = ConfigurationManager.AppSettings["curValtoGoBelow"].ToString();
            dataValue[3] = ConfigurationManager.AppSettings["waitTimeIntervalinSeconds"].ToString();
            dataValue[4] = ConfigurationManager.AppSettings["soundToPlay"].ToString();

            return dataValue;

        }

        public static async Task<decimal> getCoinValue()
        {
            string[] currencydata = getCurrencyAndValueForAlert();



            string apiUrl = "https://api.kucoin.com/api/v1/market/orderbook/level1?symbol=" + currencydata[0] +"-USDT";
            decimal coinval = 0;
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(apiUrl))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    JObject jsonResponse = JObject.Parse(apiResponse);
                    JToken usdRate = jsonResponse["data"]["price"];
                    decimal coinValue = decimal.Parse(usdRate.ToString());
                    CoinPriceData kuCoinPrice = new CoinPriceData
                    {
                        Currency = "USD",
                        Value = coinValue
                    };
                    //  Console.WriteLine($"BTC price: {kuCoinPrice.Value} {kuCoinPrice.Currency}");
                    coinval = kuCoinPrice.Value;
                }



                
            }

            return coinval;
        }


    }

    public class CoinPriceData
    {
        public string Currency { get; set; }
        public decimal Value { get; set; }
    }
}