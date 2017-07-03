﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNet4.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Drawing;

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

        // 转链，返回下载的图片路径以及文案
        public void TranslateURL(string orignalURL, string adzoneName, out string imgPath, out string showContent)
        {
            string QueryURL = "http://pub.alimama.com/items/search.json?q=";
            QueryURL += System.Web.HttpUtility.UrlEncode(orignalURL);
            HttpItem GetJuItem = new HttpItem()
            {
                URL = QueryURL,
                ContentType = "application/x-www-form-urlencoded",
                Accept = "*/*",
            };
            HttpHelper GetJuHelper = new HttpHelper();
            HttpResult GetJuResult = GetJuHelper.GetHtml(GetJuItem);
            string result = GetJuResult.Html;

            try
            {
                JObject jp1 = (JObject)JsonConvert.DeserializeObject(result);
                string title = jp1["data"]["pageList"][0]["title"].ToString();
                string price = jp1["data"]["pageList"][0]["zkPrice"].ToString();
                string coupon = jp1["data"]["pageList"][0]["couponAmount"].ToString();
                string pictureURL = jp1["data"]["pageList"][0]["pictUrl"].ToString();

                pictureURL = "http:" + pictureURL;
                DownloadImg(pictureURL, out imgPath);

                string siteId, adzoneId;
                getSiteAdzoneId(adzoneName, out siteId, out adzoneId);

                string itemid = getIdFromURL(orignalURL); // 商品id

                string TotalText = "【商品】 " + title + "\n";
                if (coupon == "0")
                {
                    // 没有优惠券
                    TotalText += "【价格】 " + price + "元！\n";
                    string order_url, coupon_token;
                    getOrderURL(itemid, siteId, adzoneId, out order_url, out coupon_token);
                    TotalText += "【口令】 " + coupon_token + "\n";
                    TotalText += "【下单】 " + order_url + "\n";
                }
                else
                {
                    // 有优惠券
                    TotalText += "【原价】 " + price + "元！\n";
                    TotalText += "【现价】 " + (float.Parse(price) - float.Parse(coupon)).ToString() + "秒杀！\n";
                    string order_url, coupon_url, coupon_token;
                    getCouponURL(itemid, siteId, adzoneId, out order_url, out coupon_url, out coupon_token);
                    TotalText += "【领券】 " + coupon_url + "\n";
                    TotalText += "【口令】 " + coupon_token + "\n";
                    TotalText += "【下单】 " + order_url + "\n";
                }
                showContent = TotalText;
            }
            catch (Exception)
            {
                imgPath = "";
                showContent = "";
            }

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

        private void DownloadImg(string imgURL, out string downloadPath)
        {
            HttpItem item = new HttpItem
            {
                URL = imgURL,
                ResultType = ResultType.Byte
            };
            HttpHelper http = new HttpHelper();
            HttpResult result = http.GetHtml(item);
            Image img = byteArrayToImage(result.ResultByte);

            string imgFileName = imgURL.Substring(imgURL.LastIndexOf('/') + 1);
            downloadPath = System.IO.Directory.GetCurrentDirectory();
            downloadPath += "\\";
            downloadPath += imgFileName;
            img.Save(downloadPath);
        }

        // 获取下单链接
        private void getOrderURL(string itemId, string siteId, string adzoneId, out string order_url, out string coupon_token)
        {
            Console.WriteLine("id = " + itemId);

            // 转链
            string newURL = "http://pub.alimama.com/common/code/getAuctionCode.json?";
            newURL += "auctionid=" + itemId;
            newURL += "&adzoneid=" + adzoneId;
            newURL += "&siteid=" + siteId;
            newURL += "&scenes=1&_input_charset=utf-8";

            string cookie = GetCookies(_frontPage);
            HttpItem GetJuItem = new HttpItem()
            {
                URL = newURL,
                ContentType = "application/x-www-form-urlencoded",
                Cookie = cookie,
                Accept = "*/*",
            };
            HttpHelper GetJuHelper = new HttpHelper();
            HttpResult GetJuResult = GetJuHelper.GetHtml(GetJuItem);
            string result = GetJuResult.Html;

            JObject jp1 = (JObject)JsonConvert.DeserializeObject(result);
            order_url = jp1["data"]["shortLinkUrl"].ToString();
            coupon_token = jp1["data"]["taoToken"].ToString();
        }

        // 获取优惠券地址
        private void getCouponURL(string itemId, string siteId, string adzoneId, out string order_url, out string coupon_url, out string coupon_token)
        {
            // 转链
            string newURL = "http://pub.alimama.com/common/code/getAuctionCode.json?";
            newURL += "auctionid=" + itemId;
            newURL += "&adzoneid=" + adzoneId;
            newURL += "&siteid=" + siteId;
            newURL += "&scenes=3&channel=tk_qqhd";

            string cookie = GetCookies(_frontPage);
            HttpItem GetJuItem = new HttpItem()
            {
                URL = newURL,
                ContentType = "application/x-www-form-urlencoded",
                Cookie = cookie,
                Accept = "*/*",
            };
            HttpHelper GetJuHelper = new HttpHelper();
            HttpResult GetJuResult = GetJuHelper.GetHtml(GetJuItem);
            string result = GetJuResult.Html;

            JObject jp1 = (JObject)JsonConvert.DeserializeObject(result);
            coupon_url = jp1["data"]["couponLink"].ToString();
            order_url = jp1["data"]["shortLinkUrl"].ToString();
            coupon_token = jp1["data"]["couponLinkTaoToken"].ToString();
        }

        private Image byteArrayToImage(byte[] Bytes)
        {
            MemoryStream ms = new MemoryStream(Bytes);
            return Bitmap.FromStream(ms, true);
        }

        private string getIdFromURL(string url)
        {
            int pos = url.IndexOf("&id=");
            if (pos == -1)
            {
                pos = url.IndexOf("?id=");
            }
            if (pos == -1)
            {
                return "";
            }

            string id = url.Substring(pos + 4);
            pos = id.IndexOf("&");
            if (pos != -1)
            {
                id = id.Substring(0, pos);
            }
            return id;
        }

        // 根据推广位名得到推广位ID和站点ID
        private void getSiteAdzoneId(string adzoneName, out string siteId, out string adzoneId)
        {
            siteId = "";
            adzoneId = "";
            foreach (Site site in _siteList)
            {
                bool bFind = false;
                foreach (Adzone adzone in site.zones)
                {
                    if (adzone.name == adzoneName)
                    {
                        Console.WriteLine("site id : " + site.id + ", adzone id : " + adzone.id);
                        siteId = site.id;
                        adzoneId = adzone.id;
                        bFind = true;
                        break;
                    }
                }
                if (bFind)
                {
                    break;
                }
            }
        }
    }
}