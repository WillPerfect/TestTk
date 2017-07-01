using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Drawing;
using System.Web;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DotNet4.Utilities;

// 产品库
namespace TkHome
{
    class ProductImageData
    {
        public string _name { get; set; }
        public HttpWebRequest _req { get; set; }
    }

    class ProductImage
    {
        public ProductImage(string name, Image img)
        {
            _name = name;
            _img = img;
        }
        public string _name { get; set; }
        public Image _img { get; set; }
    }

    class ProductInfo
    {
        public int _id { get; set; }
        public string _title { get; set; }
        public string _picURL { get; set; }
        public string _Sale30 { get; set; }
        public string _tkRate { get; set; }
        public string _tkCommFee { get; set; }
        public string _zkPrice { get; set; }
        public string _auctionUrl { get; set; }
        public string _addTime { get; set; }

        public ProductInfo(string title, string picURL, string sale30, string tkRate, string tkCommFee, string zkPrice, string auctionUrl)
        {
            _title = title;
            _picURL = picURL;
            _Sale30 = sale30;
            _tkRate = tkRate;
            _tkCommFee = tkCommFee;
            _zkPrice = zkPrice;
            _auctionUrl = auctionUrl;
        }

        public ProductInfo(int id, string title, string sale30, string tkRate, string tkCommFee, string zkPrice, string auctionUrl, string addTime)
        {
            _id = id;
            _title = title;
            _Sale30 = sale30;
            _tkRate = tkRate;
            _tkCommFee = tkCommFee;
            _zkPrice = zkPrice;
            _auctionUrl = auctionUrl;
            _addTime = addTime;
        }
    }

    class ProductLibrary
    {
        private object _insertImgLock = new object();
        private List<ProductImage> _productImageList = new List<ProductImage>();

        public List<ProductImage> GetProductImageList()
        {
            List<ProductImage> imageList = new List<ProductImage>();
            lock (_insertImgLock)
            {
                _productImageList.ForEach(t => imageList.Add(t));
                if (_productImageList.Count > 400)
                {
                    _productImageList.Clear();                    
                }
            }
            return imageList;
        }

        public List<ProductInfo> LoadProducts(int curPage, string searchText)
        {
            string strURL = "http://pub.alimama.com/items/search.json";
            if (searchText != "")
            {
                strURL += "?q=" + HttpUtility.UrlEncode(searchText) + "&";
            }
            else
            {
                strURL += "?";
            }
            strURL += "toPage=" + curPage.ToString() + "&queryType=2&auctionTag=&perPageSize=40";
            HttpItem GetJuItem = new HttpItem()
            {
                URL = strURL,
                ContentType = "application/x-www-form-urlencoded",
                Accept = "*/*"
            };
            HttpHelper GetJuHelper = new HttpHelper();
            HttpResult GetJuResult = GetJuHelper.GetHtml(GetJuItem);
            if (GetJuResult.StatusCode == HttpStatusCode.OK)
            {
                string result = GetJuResult.Html;
                return parseProducts(result);
            }
            return null;
        }

        private bool IsImageInCache(string picURL)
        {
            lock (_insertImgLock)
            {
                foreach (ProductImage item in _productImageList)
                {
                    if (item._name.Equals(picURL))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // 解析产品列表
        private List<ProductInfo> parseProducts(String jsonText)
        {
            List<ProductInfo> productList = new List<ProductInfo>();
            JObject jp = (JObject)JsonConvert.DeserializeObject(jsonText);
            JArray items = (JArray)jp["data"]["pageList"];
            foreach (var item in items)
            {
                var title = ((JObject)item)["title"];
                var picURL = ((JObject)item)["pictUrl"];
                var Sale30 = ((JObject)item)["biz30day"];
                var tkRate = ((JObject)item)["tkRate"];
                var tkCommFee = ((JObject)item)["tkCommFee"];
                var zkPrice = ((JObject)item)["zkPrice"];
                var auctionUrl = ((JObject)item)["auctionUrl"];

                if (!IsImageInCache(picURL.ToString()))
                {
                    // 异步下载图片
                    HttpWebRequest pReq = (HttpWebRequest)WebRequest.Create(new Uri("http:" + picURL));
                    ProductImageData data = new ProductImageData();
                    data._name = picURL.ToString();
                    data._req = pReq;
                    pReq.BeginGetResponse(new AsyncCallback(GetResponseCallBack), data);    
                }

                string strTitle = HttpUtility.HtmlDecode(title.ToString());
                strTitle = ReplaceHtmlTag(strTitle);

                ProductInfo info = new ProductInfo(strTitle, picURL.ToString(), Sale30.ToString(), tkRate.ToString(), tkCommFee.ToString(), zkPrice.ToString(), auctionUrl.ToString());
                productList.Add(info);
            }
            return productList;
        }
        // 异步下载图片
        public void GetResponseCallBack(IAsyncResult ia)
        {
            try
            {
                ProductImageData data = ia.AsyncState as ProductImageData;

                HttpWebResponse resp = data._req.EndGetResponse(ia) as HttpWebResponse;
                Image img1 = Image.FromStream(resp.GetResponseStream());

                ///线程锁
                lock (_insertImgLock)
                {
                    _productImageList.Add(new ProductImage(data._name, img1));
                }
            }
            catch (Exception)
            {
            }

        }

        public string ReplaceHtmlTag(string html, int length = 0)
        {
            string strText = System.Text.RegularExpressions.Regex.Replace(html, "<[^>]+>", "");
            strText = System.Text.RegularExpressions.Regex.Replace(strText, "&[^;]+;", "");

            if (length > 0 && strText.Length > length)
                return strText.Substring(0, length);

            return strText;
        }
    }
}
