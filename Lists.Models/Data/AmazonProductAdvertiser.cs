using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace Lists.Models.Data
{
    public class AmazonProductAdvertiser
    {
        private const string DESTINATION = "webservices.amazon.com";
        private const string DESTINATION_URI = "/onca/xml";
        private const string AZWS_ACCESS_KEY_ID = "<AZWS_ACCESS_KEY_ID>";
        private const string AZWS_ACCESS_KEY = "<AZWS_ACCESS_KEY>";
        private const string REQUEST_METHOD = "GET";
        private const string NAMESPACE = "http://webservices.amazon.com/AWSECommerceService/2011-08-01";

        public static void UpdateSavedProducts()
        {
            var lists = new ListContainer(true);

            foreach (var item in lists.Lists.SelectMany(l => l.Items.Items))
            {
                var productSearcher = new AmazonProductAdvertiser();
                var results = productSearcher.Search(item.Text);
                var stuffToRemove = new List<int>();

                foreach(var aItem in item.AmazonItems.Where(a => !results.Keys.Contains(a.Name)))
                {
                    aItem.UnMapFrom(item.Id);
                    stuffToRemove.Add(item.AmazonItems.Select(a => a.Id).ToList().IndexOf(aItem.Id));
                }

                for (int i = 0; i < stuffToRemove.Count; i++)
                {
                    item.AmazonItems.RemoveAt(stuffToRemove[i] - i);
                }

                foreach (var key in results.Keys)
                {
                    var amazonItem = item.AmazonItems.SingleOrDefault(i => i.Name == key);
                    if (amazonItem != null)
                    {
                        if (amazonItem.Name != key || amazonItem.Link != results[key])
                        {
                            amazonItem.Link = results[key];
                            amazonItem.Name = key;
                            amazonItem.Update();
                        }
                    }
                    else
                    {
                        amazonItem = AmazonItem.GetItem(key);

                        if (amazonItem == null)
                        {
                            amazonItem = new AmazonItem
                            {
                                Link = results[key],
                                Name = key
                            };

                            amazonItem.Save();
                        }

                        amazonItem.MapTo(item.Id);
                    }
                }

                Thread.Sleep(1000); //product results are throttled to 1 per second
            }
        }

        public Dictionary<string, string> Search(string keywords)
        {
            var parameters = new Dictionary<string, string>();
            parameters["Service"] = "AWSECommerceService";
            parameters["Operation"] = "ItemSearch";
            parameters["Condition"] = "All";
            parameters["Availability"] = "Available";
            parameters["SearchIndex"] = "All";
            parameters["Keywords"] = keywords;
            parameters["AssociateTag"] = "lstbud-20";

            string requestUrl = sign(parameters);

            var urls = new Dictionary<string, string>();

            try
            {
                var request = HttpWebRequest.Create(requestUrl);
                var response = request.GetResponse();
                var doc = new XmlDocument();
                doc.Load(response.GetResponseStream());

                var errorMessageNodes = doc.GetElementsByTagName("Message", NAMESPACE);
                if (errorMessageNodes == null || errorMessageNodes.Count == 0)
                {
                    foreach (XmlNode node in doc.GetElementsByTagName("Item", NAMESPACE))
                    {
                        string href = "";
                        string name = "";
                        foreach (XmlNode childNode in node.ChildNodes)
                        {
                            if (childNode.Name == "DetailPageURL")
                            {
                                href = childNode.InnerText;
                            }
                            else if (childNode.Name == "ItemAttributes")
                            {
                                foreach (XmlNode childSubNode in childNode.ChildNodes)
                                {
                                    if (childSubNode.Name == "Title")
                                    {
                                        name = childSubNode.InnerText;
                                        break;
                                    }
                                }
                            }

                            if (!string.IsNullOrWhiteSpace(href) && !string.IsNullOrWhiteSpace(name))
                            {
                                urls[name] = href;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //I do not care if an error occurs...continue as normal
            }

            return urls;
        }

        private string sign(Dictionary<string, string> parameters)
        {
            var sortedMap = new SortedDictionary<string, string>(parameters, new ParamComparer());

            sortedMap["Timestamp"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
            sortedMap["AWSAccessKeyId"] = AZWS_ACCESS_KEY_ID;

            string canocialQueryString = ConstructCanonicalQueryString(sortedMap);

            var builder = new StringBuilder();
            builder.Append(REQUEST_METHOD)
                   .Append("\n")
                   .Append(DESTINATION.ToLower())
                   .Append("\n")
                   .Append(DESTINATION_URI)
                   .Append("\n")
                   .Append(canocialQueryString);

            string stringToSign = builder.ToString();
            byte[] toSign = Encoding.UTF8.GetBytes(stringToSign);

            byte[] secret = Encoding.UTF8.GetBytes(AZWS_ACCESS_KEY);
            var signer = new HMACSHA256(secret);
            byte[] signatureBytes = signer.ComputeHash(toSign);
            string signature = Convert.ToBase64String(signatureBytes);

            var qsBuilder = new StringBuilder();
            qsBuilder.Append("http://")
                     .Append(DESTINATION)
                     .Append(DESTINATION_URI)
                     .Append("?")
                     .Append(canocialQueryString)
                     .Append("&Signature=")
                     .Append(PercentEncodeRfc3986(signature));

            return qsBuilder.ToString();
        }

        private string ConstructCanonicalQueryString(SortedDictionary<string, string> sortedParamMap)
        {
            StringBuilder builder = new StringBuilder();

            if (sortedParamMap.Count == 0)
            {
                builder.Append("");
                return builder.ToString();
            }

            foreach (KeyValuePair<string, string> kvp in sortedParamMap)
            {
                builder.Append(this.PercentEncodeRfc3986(kvp.Key));
                builder.Append("=");
                builder.Append(this.PercentEncodeRfc3986(kvp.Value));
                builder.Append("&");
            }
            string canonicalString = builder.ToString();
            canonicalString = canonicalString.Substring(0, canonicalString.Length - 1);
            return canonicalString;
        }

        private string PercentEncodeRfc3986(string str)
        {
            str = HttpUtility.UrlEncode(str, System.Text.Encoding.UTF8);
            str = str.Replace("'", "%27").Replace("(", "%28").Replace(")", "%29").Replace("*", "%2A").Replace("!", "%21").Replace("%7e", "~").Replace("+", "%20");

            StringBuilder sbuilder = new StringBuilder(str);
            for (int i = 0; i < sbuilder.Length; i++)
            {
                if (sbuilder[i] == '%')
                {
                    if (Char.IsLetter(sbuilder[i + 1]) || Char.IsLetter(sbuilder[i + 2]))
                    {
                        sbuilder[i + 1] = Char.ToUpper(sbuilder[i + 1]);
                        sbuilder[i + 2] = Char.ToUpper(sbuilder[i + 2]);
                    }
                }
            }
            return sbuilder.ToString();
        }
    }

    class ParamComparer : IComparer<string>
    {
        public int Compare(string p1, string p2)
        {
            return string.CompareOrdinal(p1, p2);
        }
    }
}
