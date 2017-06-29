using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TkHome
{
    public partial class MainForm : Form
    {
        private ProductLibrary _productLibrary = new ProductLibrary();
        private ProductCollector _productCollector = new ProductCollector();
        public MainForm()
        {
            InitializeComponent();
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            LoadProductLibrary(); // 加载产品库
            loadImageTimer.Start();
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            _productCollector.StopMonitor();
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
                    QQQun item = ((QQQun)collectListBox.CheckedItems[i]);
                    monitorQQWndList.Add(item.Wnd);
                }
                _productCollector.StartMonitor(monitorQQWndList); // 开始监控
                collectProductTimer.Start();
                collectButton.Text = "停止采集";
            }
            else
            {
                _productCollector.StopMonitor(); // 停止监控
                collectProductTimer.Stop();
                collectButton.Text = "开始采集";
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
            List<QQQun> wndList = _productCollector.GetAllQQQunWnd();
            collectListBox.DisplayMember = "Name";
            foreach (QQQun item in wndList)
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
                foreach (QQQun curItem in wndList)
                {
                    QQQun item = collectListBox.Items[i] as QQQun;
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
        #endregion 商品采集页面


    }
}
