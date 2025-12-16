using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Lists.Models.Data
{
    public class GroceryPrices
    {
        public GroceryPrices()
        {
        }

        public GroceryPrices(string json)
        {
            if (!string.IsNullOrWhiteSpace(json))
            {
                var jsonMap = JObject.Parse(json);
                var prices = jsonMap["results"];

                if(prices != null)
                {
                    try
                    {
                        prices = prices["prices"];
                        //multiply by 4 because milk price is in liters
                        Milk = decimal.Parse(prices[0]["milk"].ToString().Replace("$", "").Trim()) * 4;
                        Bread = decimal.Parse(prices[0]["bread"].ToString().Replace("$", "").Trim());
                        Rice = decimal.Parse(prices[0]["rice"].ToString().Replace("$", "").Trim());
                        Eggs = decimal.Parse(prices[0]["eggs"].ToString().Replace("$", "").Trim());
                        Cheese = decimal.Parse(prices[0]["cheese"].ToString().Replace("$", "").Trim());
                        Chicken = decimal.Parse(prices[0]["chicken"].ToString().Replace("$", "").Trim());
                        Apples = decimal.Parse(prices[0]["apples"].ToString().Replace("$", "").Trim());
                        Oranges = decimal.Parse(prices[0]["oranges"].ToString().Replace("$", "").Trim());
                        Potato = decimal.Parse(prices[0]["potato"].ToString().Replace("$", "").Trim());
                        Lettuce = decimal.Parse(prices[0]["lettuce"].ToString().Replace("$", "").Trim());
                    }
                    catch (Exception e)
                    {}
                }
            }
        }

        public decimal? GetPrice(string itemName)
        {
            itemName = itemName.ToLower().Trim();
            decimal? price = null;

            if (itemName.Contains("milk"))
            {
                price = Milk;
            }
            else if (itemName.Contains("bread"))
            {
                price = Bread;
            }
            else if (itemName.Contains("rice"))
            {
                price = Rice;
            }
            else if (itemName.Contains("egg"))
            {
                price = Eggs;
            }
            else if (itemName.Contains("cheese"))
            {
                price = Cheese;
            }
            else if (itemName.Contains("chicken"))
            {
                price = Chicken;
            }
            else if (itemName.Contains("apple"))
            {
                price = Apples;
            }
            else if (itemName.Contains("orange"))
            {
                price = Oranges;
            }
            else if (itemName.Contains("potato"))
            {
                price = Potato;
            }
            else if (itemName.Contains("lettuce"))
            {
                price = Lettuce;
            }

            return price;
        }

        public decimal Milk { get; set; }
        public decimal Bread { get; set; }
        public decimal Eggs { get; set; }
        public decimal Rice { get; set; }
        public decimal Cheese { get; set; }
        public decimal Chicken { get; set; }
        public decimal Apples { get; set; }
        public decimal Oranges { get; set; }
        public decimal Potato { get; set; }
        public decimal Lettuce { get; set; }
    }
}
