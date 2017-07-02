using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNet4.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TkHome
{
    class Site
    {
        public string id;
        public string name;
        public List<Adzone> zones;
    }

    class Adzone
    {
        public string id;
        public string name;
    }
    class Alimama
    {
        public static string _frontPage = "https://www.alimama.com/index.htm";
        private List<Site> _siteList = new List<Site>();

        public static bool IsOnline()
        {
            string cookie = "";

            cookie = GetCookies(_frontPage);
            // 查询是否是登录状态
            HttpItem GetJuItem = new HttpItem()
            {
                URL = "http://pub.alimama.com/common/getUnionPubContextInfo.json",
                ContentType = "application/x-www-form-urlencoded",
                Accept = "*/*",
                Cookie = cookie,
            };
            HttpHelper GetJuHelper = new HttpHelper();
            HttpResult GetJuResult = GetJuHelper.GetHtml(GetJuItem);
            string result = GetJuResult.Html;
            if (result.IndexOf("mmNick") == -1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        // 获取推广位
        public List<string> GetAdzone()
        {
            List<string> adzoneList = new List<string>();
            string cookie = "";
            cookie = GetCookies(_frontPage);
            HttpItem GetJuItem = new HttpItem()
            {
                URL = "http://pub.alimama.com/common/adzone/newSelfAdzone2.json?tag=29",
                ContentType = "application/x-www-form-urlencoded",
                Cookie = cookie,
                Accept = "*/*",
            };
            HttpHelper GetJuHelper = new HttpHelper();
            HttpResult GetJuResult = GetJuHelper.GetHtml(GetJuItem);
            string result = GetJuResult.Html;

            parseAdzone(result, adzoneList);
            return adzoneList;
        }

        private void parseAdzone(String jsonText, List<string> adzoneList)
        {
            JObject jp2 = (JObject)JsonConvert.DeserializeObject(jsonText);
            JArray adzones = (JArray)jp2["data"]["otherAdzones"];
            foreach (var item in adzones)
            {
                var id = ((JObject)item)["id"];
                var name = ((JObject)item)["name"];

                Console.WriteLine("siteid : " + id + "\t" + name.ToString());

                Site site = new Site();
                site.id = id.ToString();
                site.name = name.ToString();
                site.zones = new List<Adzone>();

                try
                {
                    JArray subs = (JArray)((JObject)item)["sub"];
                    foreach (var subitem in subs)
                    {
                        var subid = ((JObject)subitem)["id"];
                        var subname = ((JObject)subitem)["name"];
                        Console.WriteLine("adzoneid : " + subid + "\t" + subname.ToString());
                        adzoneList.Add(subname.ToString());

                        Adzone adzone = new Adzone();
                        adzone.id = subid.ToString();
                        adzone.name = subname.ToString();
                        site.zones.Add(adzone);
                    }
                }
                catch (Exception)
                {

                }

                _siteList.Add(site);
            }
        }

        private static string GetCookies(string url)
        {
            try
            {
                uint datasize = 1024;
                StringBuilder cookieData = new StringBuilder((int)datasize);
                if (!Win32.InternetGetCookieEx(url, null, cookieData, ref datasize, 0x2000, IntPtr.Zero))
                {
                    if (datasize < 0)
                        return null;

                    cookieData = new StringBuilder((int)datasize);
                    if (!Win32.InternetGetCookieEx(url, null, cookieData, ref datasize, 0x00002000, IntPtr.Zero))
                        return null;
                }
                return cookieData.ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
