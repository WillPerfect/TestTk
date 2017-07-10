using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace TkHome
{
    public partial class MainForm : Form
    {
        private ProductLibrary _productLibrary = new ProductLibrary();
        private ProductCollector _productCollector = new ProductCollector();
        private DbOperator _dbOperator = new DbOperator();
        private ProductQunfa _productQunfa = new ProductQunfa();
        private Alimama _alimama = new Alimama();
        private int _productCountPerPage = 100; // 自选库中每页显示的商品数
        private bool _startQunfa = false;
        public MainForm()
        {
            InitializeComponent();
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            LoadProductLibrary(); // 加载产品库
            loadImageTimer.Start();
            _dbOperator.init();
            LoadMyLibrary(); // 加载自选库
            initCollectConfigure();
            initQunfaConfigure();
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            _productCollector.StopMonitor();
            _productQunfa.StopTranslateUrl();
            _dbOperator.deinit();
        }

        #region 产品库页面
        private void OnLoadImageTimer(object sender, EventArgs e)
        {
            // 定时获取产品图片
           List<ProductImage> imageList = _productLibrary.GetProductImageList();
           foreach (ProductImage item in imageList)
           {
               if (productLibraryImageList.Images.IndexOfKey(item._name) == -1)
               {
                   productLibraryImageList.Images.Add(item._name, item._img);                
               }
           }
        }

        // 上一页
        private void lastPage1_Click(object sender, EventArgs e)
        {
            int curPage = int.Parse(pageLabel1.Text);
            if (curPage > 1)
            {
                curPage--;
                LoadProductLibrary();
                pageLabel1.Text = curPage.ToString();
            }
        }

        // 下一页
        private void nextPage1_Click(object sender, EventArgs e)
        {
            int curPage = int.Parse(pageLabel1.Text);
            curPage++;
            LoadProductLibrary();
            pageLabel1.Text = curPage.ToString();            
        }

        // 搜索
        private void searchButton_Click(object sender, EventArgs e)
        {
            pageLabel1.Text = "1";
            LoadProductLibrary();
        }

        // 清空搜索
        private void clearSearchButton_Click(object sender, EventArgs e)
        {
            pageLabel1.Text = "1";
            searchTextBox.Text = "";
            LoadProductLibrary();
        }

        // 加载产品库
        private void LoadProductLibrary()
        {
            productListView.Items.Clear();
            productLibraryImageList.Images.Clear();

            int curPage = int.Parse(pageLabel1.Text);
            string searchText = searchTextBox.Text;
            List<ProductInfo> prodcutList = _productLibrary.LoadProducts(curPage, searchText);
            if (prodcutList != null)
            {
                foreach (ProductInfo item in prodcutList)
                {
                    ListViewItem lv = productListView.Items.Add(item._title);
                    lv.SubItems.Add(item._zkPrice);
                    lv.SubItems.Add(item._tkRate + "%");
                    lv.SubItems.Add(item._tkCommFee);
                    lv.SubItems.Add(item._Sale30);
                    lv.ImageKey = item._picURL;
                    lv.Tag = item._auctionUrl;
                }
            }
        }

        // 添加到自选库
        private void addToMyLibrary1_Click(object sender, EventArgs e)
        {
            List<ProductInfo> selectedProductList = new List<ProductInfo>();
            foreach (ListViewItem item in productListView.SelectedItems)
            {
                selectedProductList.Add(new ProductInfo(item.SubItems[0].Text, "", item.SubItems[4].Text, item.SubItems[2].Text, item.SubItems[3].Text, item.SubItems[1].Text, item.Tag as string));
            }
            _dbOperator.addProductList(selectedProductList);
        }
        #endregion 产品库页面

        #region 商品采集页面
        // 开始采集
        private void collectButton_Click(object sender, EventArgs e)
        {
            if (!collectProductTimer.Enabled)
            {
                List<int> monitorQQWndList = new List<int>();
                for (int i = 0; i < collectListBox.CheckedItems.Count; i++)
                {
                    WndInfo item = ((WndInfo)collectListBox.CheckedItems[i]);
                    monitorQQWndList.Add(item.Wnd);
                }
                int startTime, endTime, interval;
                _dbOperator.loadCollectConfigure(out startTime, out endTime, out interval);
                _productCollector.StartMonitor(monitorQQWndList, startTime, endTime, interval); // 开始监控
                collectProductTimer.Start();
                collectButton.Text = "停止采集";
                collectListBox.Enabled = false;
                refreshCollectButton.Enabled = false;
            }
            else
            {
                _productCollector.StopMonitor(); // 停止监控
                collectProductTimer.Stop();
                collectButton.Text = "开始采集";
                collectListBox.Enabled = true;
                refreshCollectButton.Enabled = true;
            }
        }

        // 采集定时器
        private void OnCollectProductTimer(object sender, EventArgs e)
        {
            List<CollectURL> collectURLList = _productCollector.GetCollectedURL();
            foreach (CollectURL item in collectURLList)
            {
                if (collectListView.Items.Count == 0 || collectListView.FindItemWithText(item.Title, false, 0, false) == null) // 没有找到则加入
                {
                    ListViewItem lv = collectListView.Items.Add(item.Title);
                    lv.SubItems.Add(item.ZkPrice);
                    lv.SubItems.Add(item.TkRate);
                    lv.SubItems.Add(item.TkCommFee);
                    lv.SubItems.Add(item.Sale30);
                    lv.SubItems.Add(item.Time);
                    lv.Tag = item.OriginURL;
                }
                collectProductLabel.Text = "已采集商品(" + collectListView.Items.Count.ToString() + ")";
            }
        }

        // 刷新QQ群
        private void refreshCollectButton_Click(object sender, EventArgs e)
        {
            List<WndInfo> wndList = _productCollector.GetAllQQQunWnd();
            collectListBox.DisplayMember = "Name";
            foreach (WndInfo item in wndList)
            {
                if (collectListBox.FindString(item.Name) == -1)
                {
                    collectListBox.Items.Add(item);
                }
            }
            // 删除项
            for (int i = collectListBox.Items.Count - 1; i >= 0; i--)
            {
                bool bFind = false;
                foreach (WndInfo curItem in wndList)
                {
                    WndInfo item = collectListBox.Items[i] as WndInfo;
                    if (curItem.Name == item.Name)
                    {
                        bFind = true;
                        break;
                    }
                }
                if (!bFind)
                {
                    collectListBox.Items.RemoveAt(i);
                }
            }
        }
        // 添加到自选库
        private void addToMyLibrary2_Click(object sender, EventArgs e)
        {
            List<ProductInfo> selectedProductList = new List<ProductInfo>();
            foreach (ListViewItem item in collectListView.SelectedItems)
            {
                selectedProductList.Add(new ProductInfo(item.SubItems[0].Text, "", item.SubItems[4].Text, item.SubItems[2].Text, item.SubItems[3].Text, item.SubItems[1].Text, item.Tag as string));
            }
            _dbOperator.addProductList(selectedProductList);
        }

        // 删除采集项
        private void delCollectButton_Click(object sender, EventArgs e)
        {
            for (int i = collectListView.SelectedItems.Count - 1; i >= 0; i--)
            {
                ListViewItem lv = collectListView.SelectedItems[i];
                collectListView.Items.Remove(lv);
            }
        }

        // 清空采集项
        private void clearCollectButton_Click(object sender, EventArgs e)
        {
            collectListView.Items.Clear();
        }
        #endregion 商品采集页面

        #region 自选库页面
        // 加载自选库
        private void LoadMyLibrary()
        {
            MyLibraryListView.Items.Clear();

            int curPage = int.Parse(pageLabel2.Text);
            int startRow = (curPage - 1) * _productCountPerPage;
            List<ProductInfo> productList = _dbOperator.loadProductList(startRow, _productCountPerPage);
            foreach (ProductInfo product in productList)
            {
                ListViewItem lv = MyLibraryListView.Items.Add(product._id.ToString());
                lv.SubItems.Add(product._title);
                lv.SubItems.Add(product._zkPrice);
                lv.SubItems.Add(product._tkRate);
                lv.SubItems.Add(product._tkCommFee);
                lv.SubItems.Add(product._addTime);
            }
        }

        // 刷新自选库
        private void refreshMyLibraryButton_Click(object sender, EventArgs e)
        {
            LoadMyLibrary();
        }

        // 自选库上一页
        private void lastPage2_Click(object sender, EventArgs e)
        {
            int curPage = int.Parse(pageLabel2.Text);
            if (curPage > 1)
            {
                pageLabel2.Text = (curPage - 1).ToString();
            }
            LoadMyLibrary();
        }

        // 自选库下一页
        private void nextPage2_Click(object sender, EventArgs e)
        {
            int curPage = int.Parse(pageLabel2.Text);
            pageLabel2.Text = (curPage + 1).ToString();
            LoadMyLibrary();
        }

        // 删除自选库商品
        private void delMyLibraryItemButton_Click(object sender, EventArgs e)
        {
            List<int> productIdList = new List<int>();
            foreach (ListViewItem item in MyLibraryListView.SelectedItems)
            {
                productIdList.Add(Convert.ToInt32(item.Text));
            }
            _dbOperator.delProductIdList(productIdList);
            LoadMyLibrary();
        }
        #endregion 自选库页面

        #region 商品群发页面

        // 刷新QQ微信群
        private void refreshSendButton_Click(object sender, EventArgs e)
        {
            List<WndInfo> wndList = _productQunfa.GetAllQQWechatWnd();
            qunfaListBox.DisplayMember = "Name";
            foreach (WndInfo item in wndList)
            {
                if (qunfaListBox.FindString(item.Name) == -1)
                {
                    qunfaListBox.Items.Add(item);
                }
            }
            // 删除项
            for (int i = qunfaListBox.Items.Count - 1; i >= 0; i--)
            {
                bool bFind = false;
                foreach (WndInfo curItem in wndList)
                {
                    WndInfo item = qunfaListBox.Items[i] as WndInfo;
                    if (curItem.Name == item.Name)
                    {
                        bFind = true;
                        break;
                    }
                }
                if (!bFind)
                {
                    qunfaListBox.Items.RemoveAt(i);
                }
            }
        }

        // 开始群发
        private void qunfaButton_Click_1(object sender, EventArgs e)
        {
            if (!_startQunfa)
            {
                qunfaButton.Text = "停止群发";
                _startQunfa = true;
                int startTime, endTime, interval;
                _dbOperator.loadQunfaConfigure(out startTime, out endTime, out interval);
                _productQunfa.StartTranslateUrl(_dbOperator, _alimama, adzoneComboBox.Text, startTime, endTime, interval);
                qunfaTimer.Start();
                qunfaListBox.Enabled = false;
                refreshSendButton.Enabled = false;
            }
            else
            {
                qunfaButton.Text = "开始群发";
                _startQunfa = false;
                qunfaTimer.Stop();
                _productQunfa.StopTranslateUrl();
                qunfaListBox.Enabled = true;
                refreshSendButton.Enabled = true;
            }
        }

        // 群发定时器
        private void OnQunfaTimer(object sender, EventArgs e)
        {
            List<WndInfo> qunfaWndList = new List<WndInfo>();
            for (int i = 0; i < qunfaListBox.CheckedItems.Count; i++)
            {
                WndInfo item = ((WndInfo)qunfaListBox.CheckedItems[i]);
                qunfaWndList.Add(item);
            }
            List<TranslateUrlResult> resultList = _productQunfa.GetTranslateResult();
            foreach (TranslateUrlResult result in resultList)
            {
                //QQ群发
                MemoryStream ms = new MemoryStream(System.Text.Encoding.Default.GetBytes(result.QQShowContent));// copy
                Clipboard.SetData("QQ_RichEdit_Format", ms);

                foreach (WndInfo wnd in qunfaWndList)
                {
                    try
                    {
                        if (wnd.IsQQWnd) // 发送到QQ窗口
                        {
                            if (Win32.IsIconic(wnd.Wnd))
                            {
                                Win32.ShowWindow(wnd.Wnd, Win32.SW_RESTORE); // 如果QQ窗口最小化，则恢复
                            }

                            Win32.SendMessage(wnd.Wnd, Win32.WM_PASTE, 0, 0); // paste

                            Win32.SendMessage(wnd.Wnd, Win32.WM_KEYDOWN, Win32.VK_RETURN, 0); // send

                            Win32.ShowWindow(wnd.Wnd, Win32.SW_MINIMIZE); // 最小化QQ窗口
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("qunfa exception : " + ex.Message);
                    }

                    Thread.Sleep(1000);
                }
                Clipboard.Clear();

                //微信群发
                Clipboard.SetData(DataFormats.Html, result.WechatShowContent);// copy
                foreach (WndInfo wnd in qunfaWndList)
                {
                    try
                    {
                        if (!wnd.IsQQWnd) // 发送到微信窗口
                        {
                            int pos = 0x023a0113; // 275, 570

                            // paste
                            Win32.SendMessage(wnd.Wnd, Win32.WM_LBUTTONDOWN, Win32.MK_LBUTTON, pos);
                            Thread.Sleep(10);
                            Win32.SendMessage(wnd.Wnd, Win32.WM_LBUTTONUP, 0, pos);

                            Win32.keybd_event(Win32.VK_CONTROL, 0, 0, 0);
                            Win32.SendMessage(wnd.Wnd, Win32.WM_KEYDOWN, 'V', 0);
                            Win32.SendMessage(wnd.Wnd, Win32.WM_KEYUP, 'V', 0);
                            Win32.keybd_event(Win32.VK_CONTROL, 0, Win32.KEYEVENTF_KEYUP, 0);

                            Win32.SendMessage(wnd.Wnd, Win32.WM_KEYDOWN, Win32.VK_RETURN, 0); // send
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("qunfa exception : " + ex.Message);
                    }

                    Thread.Sleep(1000);
                }
                Clipboard.Clear();

                ListViewItem lv = qunfaListView.Items.Add(result.ProductTitle);
                lv.SubItems.Add(DateTime.Now.ToString());
            }
        }

        // 清空群发列表
        private void clearQunfaButton_Click(object sender, EventArgs e)
        {
            qunfaListView.Items.Clear();
        }
        #endregion 商品群发页面

        #region 实用工具页面
        // 转链
        private void translateButton_Click(object sender, EventArgs e)
        {
            if (!Alimama.IsOnline())
            {
                MessageBox.Show("检测到掉线，正在尝试重连，请稍后重试...");
            }
            else
            {
                string url = urlTextBox.Text;
                if (url == "")
                {
                    return;
                }
                string adzoneName = adzoneComboBox.Text;
                string imagePath, showContent;
                _alimama.TranslateURL(url, adzoneName, out imagePath, out showContent);

                TranslateLinkForm tlFrom = new TranslateLinkForm();
                tlFrom.ImagePath = imagePath;
                tlFrom.ShowText = showContent;
                tlFrom.ShowDialog();
            }
        }
        #endregion 实用工具页面

        #region 登录阿里妈妈页面

        // 登录阿里妈妈
        private void loginAliButton_Click_1(object sender, EventArgs e)
        {
            WebForm webForm = new WebForm();
            webForm.ShowDialog();
            if (webForm.IsLogined)
            {
                loginAliButton.Enabled = false;
                getAdzoneButton.Enabled = true;
            }
        }

        // 登录后获取推广位
        private void getAdzoneButton_Click(object sender, EventArgs e)
        {
            if (!Alimama.IsOnline())
            {
                MessageBox.Show("检测到掉线，正在尝试重连，请稍后重试...");
            }
            else
            {
                List<string> adzoneList = _alimama.GetAdzone();
                foreach (string item in adzoneList)
                {
                    adzoneComboBox.Items.Add(item);
                }
                if (adzoneComboBox.Items.Count > 0)
                {
                    adzoneComboBox.SelectedIndex = 0;
                    qunfaButton.Enabled = true;
                    translateButton.Enabled = true;
                }
            }
        }

        private void initCollectConfigure()
        {
            int startTime, endTime, interval;
            _dbOperator.loadCollectConfigure(out startTime, out endTime, out interval);
            collectStartUpDown.Value = startTime;
            collectEndUpDown.Value = endTime;
            collectIntervalUpDown.Value = interval;
        }

        private void initQunfaConfigure()
        {
            int startTime, endTime, interval;
            _dbOperator.loadQunfaConfigure(out startTime, out endTime, out interval);
            qunfaStartUpDown.Value = startTime;
            qunfaEndUpDown.Value = endTime;
            qunfaIntervalUpDown.Value = interval;
        }

        // 保存采集设置
        private void saveCollectButton_Click(object sender, EventArgs e)
        {
            int startTime = Convert.ToInt32(collectStartUpDown.Value);
            int endTime = Convert.ToInt32(collectEndUpDown.Value);
            if (startTime >= endTime)
            {
                MessageBox.Show("起始时间必须小于结束时间");
                return;
            }
            int interval = Convert.ToInt32(collectIntervalUpDown.Value);
            _dbOperator.saveCollectConfigure(startTime, endTime, interval);
        }


        // 保存群发设置
        private void saveQunfaButton_Click(object sender, EventArgs e)
        {
            int startTime = Convert.ToInt32(qunfaStartUpDown.Value);
            int endTime = Convert.ToInt32(qunfaEndUpDown.Value);
            if (startTime >= endTime)
            {
                MessageBox.Show("起始时间必须小于结束时间");
                return;
            }
            int interval = Convert.ToInt32(qunfaIntervalUpDown.Value);
            _dbOperator.saveQunfaConfigure(startTime, endTime, interval);
        }
        #endregion
    }
}
