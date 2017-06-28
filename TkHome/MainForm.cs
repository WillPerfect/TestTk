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
        public MainForm()
        {
            InitializeComponent();
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            LoadProductLibrary(); // 加载产品库
            loadImageTimer.Start();
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
                    lv.SubItems.Add(item._auctionUrl);
                    lv.ImageKey = item._picURL;
                }
            }
        }
        #endregion 产品库页面
    }
}
