using Lists.Constants;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Lists.Models.Data
{
    public class IpAddressSalesTax
    {
        public string IpAddress { get; set; }
        public long EntryId { get; set; }
        public string ZipCode { get; set; }
        public decimal? SalesTax { get; set; }

        public IpAddressSalesTax() { }
        public IpAddressSalesTax(int entryId)
        {
            Load(entryId);
        }

        public static void UpdateIpAddressSalesTax()
        {
            var items = GetAllWithNoTax();

            foreach (var item in items)
            {
                var regionInfo = ScrapeRegionInfoByIpAddress(item.IpAddress);
                decimal? salesTax = null;
                string zipCode = null;

                if (regionInfo != null)
                {
                    if (regionInfo.Item1 != null)
                    {
                        zipCode = regionInfo.Item1;
                        salesTax = ScrapeSalesTaxByZipCode(regionInfo.Item1);
                    }
                    else if (regionInfo.Item2 != null && regionInfo.Item3 != null)
                    {
                        string stateCode = GetStateCode(regionInfo.Item2);
                        if (stateCode != null)
                        {
                            zipCode = ScrapeZipCodeByStateCity(stateCode, regionInfo.Item3);
                            salesTax = ScrapeSalesTaxByZipCode(zipCode);
                        }
                    }
                }

                item.SalesTax = salesTax;
                item.ZipCode = zipCode;
                item.Update();
                Thread.Sleep(1000); //wait a little bit to prevent overwelming sites with requests
            }
        }

        public static string ScrapeZipCodeByStateCity(string stateCode, string city)
        {
            string requestUrl = "http://www.getzips.com/cgi-bin/ziplook.exe?What=2&City={0}&State={1}&Submit=Look+It+Up";
            string html = "";
            using (var client = new WebClient())
            {
                client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.65 Safari/537.36");
                client.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
                client.Headers.Add("Accept-Language", "en-US,en;q=0.8");
                client.Headers.Add("Cache-Control", "max-age=0");
                client.Headers.Add("Cookie", "id=gs; __utma=85384463.557037913.1417387637.1417489041.1417926567.3; __utmz=85384463.1417926567.3.3.utmcsr=google|utmccn=(organic)|utmcmd=organic|utmctr=(not%20provided)");
                client.Headers.Add("Host", "www.getzips.com");

                using (var data = client.OpenRead(string.Format(requestUrl, city, stateCode)))
                {
                    using (var reader = new StreamReader(data))
                    {
                        html = reader.ReadToEnd();
                    }
                }
            }

            string zipCode = null;

            if(!string.IsNullOrWhiteSpace(html))
            {
                string regexPattern = @"\b\d{5}\b";
                var matches = Regex.Match(html, regexPattern);
                string parsedSalesTax = matches.Value;
                if (matches.Success)
                {
                    zipCode = matches.Groups[0].Value;
                }
            }

            return zipCode;
        }

        public static decimal ScrapeSalesTaxByZipCode(string zipCode)
        {
            string html = "";
            using(var client = new WebClient())
            {
                client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.65 Safari/537.36");
                client.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
                client.Headers.Add("Accept-Language", "en-US,en;q=0.8");
                client.Headers.Add("Cache-Control", "max-age=0");
                client.Headers.Add("Cookie", "comm100standby_session49685=-21508; Zip2Tax=CookieID=5754131; __qca=P0-1576407645-1416712331439; optimizelySegments=%7B%7D; optimizelyEndUserId=oeu1416712352747r0.9343738730531186; optimizelyBuckets=%7B%222124614001%22%3A%222130432481%22%7D; Z2Tv25=tid=2901313; ASPSESSIONIDASTBRBBS=NLLAIHIBOFKCIFOEFOFPDMIL; _ga=GA1.2.1465038188.1416712331; comm100standby_session49685=-22917; comm100_guid_49685=53c4707e59d34144b1e50a0d2e02a26e");
                client.Headers.Add("Host", "www.zip2tax.com");

                using (var data = client.OpenRead(string.Format("http://www.zip2tax.com/Website/pagesTaxRates/z2t_lookup.asp?inputZip={0}", zipCode)))
                {
                    using (var reader = new StreamReader(data))
                    {
                        html = reader.ReadToEnd();
                    }
                }
            }

            decimal salesTax = 7.00M;

            if (!string.IsNullOrWhiteSpace(html))
            {
                string regexPattern = "<span ID=\\\"result_rate\\\".+>(.*?)</span>";
                string numberPattern = @">\d+(\.(\d)+|)<";
                var matches = Regex.Match(html, regexPattern);
                string parsedSalesTax = matches.Value;
                if (matches.Success)
                {
                    var numberMatch = Regex.Match(matches.Value, numberPattern);
                    string number =  "";
                    if (numberMatch.Success)
                    {
                        number = numberMatch.Value.Replace(">", "").Replace("<", "");
                        decimal.TryParse(number, out salesTax);
                    }
                }
            }

            return salesTax / 100.00M;
        }

        public static Tuple<string, string, string> ScrapeRegionInfoByIpAddress(string ipAddress)
        {
            var client = new WebClient();
            Tuple<string, string, string> regionInfo = null;
            using(var data = client.OpenRead(string.Format("http://ipinfo.io/{0}/json", ipAddress)))
            {
                using(var reader = new StreamReader(data))
                {
                    string json = reader.ReadToEnd();
                    dynamic parsedJson = JsonConvert.DeserializeObject(json);
                    if(parsedJson != null && parsedJson.country.ToString() == "US")
                    {
                        string postalCode = null;
                        if (parsedJson.postal != null)
                        {
                            postalCode = parsedJson.postal.ToString();
                        }

                        string region = null;
                        if (parsedJson.region != null)
                        {
                            region = parsedJson.region.ToString();
                        }

                        string city = null;
                        if (parsedJson.city != null)
                        {
                            city = parsedJson.city.ToString();
                        }

                        regionInfo = Tuple.Create<string, string, string>(postalCode, region, city);
                    }
                }
            }
            return regionInfo;
        }

        private static string GetStateCode(string stateName)
        {
            string code = null;

            using (var command = new SqlCommand(SQL.GetStateCodeByStateName))
            {
                command.Parameters.AddWithValue("@state_name", stateName);
                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            code = reader.GetString(0);
                        }
                        reader.Close();
                    }
                    databaseConnection.Close();
                }
            }

            return code;
        }

        public static string GetZipCodeByCityAndState(string city, string state)
        {
            string uspsUsername = "505LSTBU1038";
            string uspsApiUrl = "http://production.shippingapis.com/ShippingAPI.dll?API=ZipCodeLookup&XML=<ZipCodeLookupRequest%20USERID=\"{0}\"><Address ID=\"1\"><Address1></Address1><Address2></Address2><City>{1}</City><State>{2}</State><Zip5></Zip5><Zip4></Zip4></Address></ZipCodeLookupRequest>";
            string requestUrl = string.Format(uspsApiUrl, uspsUsername, city, state);

            var client = new WebClient();
            string zipCode = null;
            using (var data = client.OpenRead(requestUrl))
            {
                using (var reader = new StreamReader(data))
                {
                    string json = reader.ReadToEnd();
                    var doc = new XmlDocument();
                    doc.LoadXml(json);
                    json = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.None, true); //convert xml to json to parse
                    dynamic parsedJson = JsonConvert.DeserializeObject(json);
                    if (parsedJson != null && 
                        parsedJson.Address != null)
                    {
                        if (parsedJson.Address is IEnumerable<dynamic>)
                        {
                            zipCode = parsedJson.Address[0].Zip5.ToString();
                        }
                        else
                        {
                            zipCode = parsedJson.Address.Zip5.ToString();
                        }
                    }
                }
            }

            return zipCode;
        }

        public static List<IpAddressSalesTax> GetAllWithNoTax()
        {
            var salesTaxes = new List<IpAddressSalesTax>();

            using (var command = new SqlCommand(SQL.GetIpAddressWithNoSalesTax))
            {
                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var item = new IpAddressSalesTax
                            {
                                EntryId = reader.GetInt64(reader.GetOrdinal("entry_id")),
                                IpAddress = reader.GetString(reader.GetOrdinal("ip_address"))
                            };
                            
                            if (!reader.IsDBNull(reader.GetOrdinal("zip_code")))
                            {
                                item.ZipCode = reader.GetString(reader.GetOrdinal("zip_code"));
                            }

                            salesTaxes.Add(item);
                        }
                        reader.Close();
                    }
                    databaseConnection.Close();
                }
            }

            return salesTaxes;
        }

        public static IpAddressSalesTax GetByIpAddress(string ipAddress, bool hasSalesTax = false)
        {
            var item = new IpAddressSalesTax();

            using (var command = new SqlCommand(SQL.GetIpAddressSalesTaxByIp))
            {
                command.Parameters.AddWithValue("@ip_address", ipAddress);
                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            item.EntryId = reader.GetInt64(reader.GetOrdinal("entry_id"));
                            item.IpAddress = reader.GetString(reader.GetOrdinal("ip_address"));

                            if (!reader.IsDBNull(reader.GetOrdinal("zip_code")))
                            {
                                item.ZipCode = reader.GetString(reader.GetOrdinal("zip_code"));
                            }

                            if (!reader.IsDBNull(reader.GetOrdinal("sales_tax")))
                            {
                                item.SalesTax = reader.GetDecimal(reader.GetOrdinal("sales_tax"));
                            }
                        }
                        reader.Close();
                    }
                    databaseConnection.Close();
                }
            }

            return item;
        }

        public static IpAddressSalesTax GetByZipCode(string zipCode)
        {
            var item = new IpAddressSalesTax();

            using (var command = new SqlCommand(SQL.GetIpAddressSalesTaxByZipCode))
            {
                command.Parameters.AddWithValue("@zip_code", zipCode);
                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            item.EntryId = reader.GetInt64(reader.GetOrdinal("entry_id"));
                            item.IpAddress = reader.GetString(reader.GetOrdinal("ip_address"));

                            if (!reader.IsDBNull(reader.GetOrdinal("zip_code")))
                            {
                                item.ZipCode = reader.GetString(reader.GetOrdinal("zip_code"));
                            }

                            if (!reader.IsDBNull(reader.GetOrdinal("sales_tax")))
                            {
                                item.SalesTax = reader.GetDecimal(reader.GetOrdinal("sales_tax"));
                            }
                        }
                        reader.Close();
                    }
                    databaseConnection.Close();
                }
            }

            return item;
        }

        public void Load(long entryId)
        {
            EntryId = entryId;
            using (var command = new SqlCommand(SQL.GetIpAddressSalesTaxById))
            {
                command.Parameters.AddWithValue("@entry_id", entryId);

                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            IpAddress = reader.GetString(reader.GetOrdinal("ip_address"));

                            if (!reader.IsDBNull(reader.GetOrdinal("zip_code")))
                            {
                                ZipCode = reader.GetString(reader.GetOrdinal("zip_code"));
                            }

                            if (!reader.IsDBNull(reader.GetOrdinal("sales_tax")))
                            {
                                SalesTax = reader.GetDecimal(reader.GetOrdinal("sales_tax"));
                            }
                        }
                        reader.Close();
                    }
                    databaseConnection.Close();
                }
            }
        }

        public void Save()
        {
            using (var command = new SqlCommand(SQL.SaveIpAddressSalesTax))
            {
                command.Parameters.AddWithValue("@ip_address", IpAddress);

                if (string.IsNullOrWhiteSpace(ZipCode))
                {
                    command.Parameters.AddWithValue("@zip_code", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@zip_code", ZipCode);
                }

                if (SalesTax.HasValue)
                {

                    command.Parameters.AddWithValue("@sales_tax", SalesTax.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@sales_tax", DBNull.Value);
                }

                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            EntryId = reader.GetInt64(0);
                        }
                        reader.Close();
                    }
                    databaseConnection.Close();
                }
            }
        }

        public void Update()
        {
            using (var command = new SqlCommand(SQL.UpdateIpAddressSalesTax))
            {
                command.Parameters.AddWithValue("@ip_address", IpAddress);
                command.Parameters.AddWithValue("@entry_id", EntryId);

                if (string.IsNullOrWhiteSpace(ZipCode))
                {
                    command.Parameters.AddWithValue("@zip_code", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@zip_code", ZipCode);
                }

                if (SalesTax.HasValue)
                {

                    command.Parameters.AddWithValue("@sales_tax", SalesTax.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@sales_tax", DBNull.Value);
                }

                using (var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[AppSettings.SiteDbConnection].ConnectionString))
                {
                    command.Connection = databaseConnection;
                    databaseConnection.Open();

                    command.ExecuteNonQuery();
                    databaseConnection.Close();
                }
            }
        }
    }
}
