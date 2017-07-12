using System;
using System.Collections;
using System.Windows.Forms;

namespace Common
{
    public enum OrderType
    {
        ORDER_BY_TEXT,
        ORDER_BY_INT,
        ORDER_BY_TIME
    }
    /// <summary>
    /// 对ListView点击列标题自动排序功能
    /// </summary>
    public class ListViewHelper
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ListViewHelper()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }
        public static void ListView_ColumnClick(object sender, System.Windows.Forms.ColumnClickEventArgs e)
        {
            System.Windows.Forms.ListView lv = sender as System.Windows.Forms.ListView;

            // 检查点击的列是不是现在的排序列.
            if (e.Column == (lv.ListViewItemSorter as ListViewColumnSorter).SortColumn)
            {
                // 重新设置此列的排序方法.
                if ((lv.ListViewItemSorter as ListViewColumnSorter).Order == System.Windows.Forms.SortOrder.Ascending)
                {
                    (lv.ListViewItemSorter as ListViewColumnSorter).Order = System.Windows.Forms.SortOrder.Descending;
                }
                else
                {
                    (lv.ListViewItemSorter as ListViewColumnSorter).Order = System.Windows.Forms.SortOrder.Ascending;
                }
            }
            else
            {
                // 设置排序列，默认为正向排序
                if (lv.Columns[e.Column].Text == "标题")
                {
                    (lv.ListViewItemSorter as ListViewColumnSorter).SortType = OrderType.ORDER_BY_TEXT;
                }
                else if (lv.Columns[e.Column].Text.IndexOf("时间") != -1)
                {
                    (lv.ListViewItemSorter as ListViewColumnSorter).SortType = OrderType.ORDER_BY_TIME;
                }
                else
                {
                    (lv.ListViewItemSorter as ListViewColumnSorter).SortType = OrderType.ORDER_BY_INT;
                }
                (lv.ListViewItemSorter as ListViewColumnSorter).SortColumn = e.Column;
                (lv.ListViewItemSorter as ListViewColumnSorter).Order = System.Windows.Forms.SortOrder.Ascending;
            }
            // 用新的排序方法对ListView排序
            ((System.Windows.Forms.ListView)sender).Sort();
            ((System.Windows.Forms.ListView)sender).SetSortIcon((lv.ListViewItemSorter as ListViewColumnSorter).SortColumn, (lv.ListViewItemSorter as ListViewColumnSorter).Order);
        }
    }
    /// <summary>
    /// 继承自IComparer
    /// </summary>
    public class ListViewColumnSorter : System.Collections.IComparer
    {
        /// <summary>
        /// 指定按照哪个列排序
        /// </summary>
        private int ColumnToSort;
        /// <summary>
        /// 指定排序的方式
        /// </summary>
        private System.Windows.Forms.SortOrder OrderOfSort;
        /// <summary>
        /// 声明CaseInsensitiveComparer类对象
        /// </summary>
        private System.Collections.CaseInsensitiveComparer ObjectCompare;
        /// <summary>
        /// 构造函数
        /// </summary>
        public ListViewColumnSorter()
        {
            // 默认按第一列排序
            ColumnToSort = 0;
            // 排序方式为不排序
            OrderOfSort = System.Windows.Forms.SortOrder.None;
            // 初始化CaseInsensitiveComparer类对象
            ObjectCompare = new System.Collections.CaseInsensitiveComparer();
        }
        /// <summary>
        /// 重写IComparer接口.
        /// </summary>
        /// <param name="x">要比较的第一个对象</param>
        /// <param name="y">要比较的第二个对象</param>
        /// <returns>比较的结果.如果相等返回0，如果x大于y返回1，如果x小于y返回-1</returns>
        public int Compare(object x, object y)
        {
            int compareResult;
            System.Windows.Forms.ListViewItem listviewX, listviewY;
            // 将比较对象转换为ListViewItem对象
            listviewX = (System.Windows.Forms.ListViewItem)x;
            listviewY = (System.Windows.Forms.ListViewItem)y;
            string xText = listviewX.SubItems[ColumnToSort].Text;
            string yText = listviewY.SubItems[ColumnToSort].Text;

            if (SortType == OrderType.ORDER_BY_TEXT)
            {
                compareResult = String.Compare(xText, yText);
            }
            else if (SortType == OrderType.ORDER_BY_INT)
            {
                if (xText.EndsWith("%"))
                {
                    xText = xText.Substring(0, xText.Length - 1);
                }
                if (yText.EndsWith("%"))
                {
                    yText = yText.Substring(0, yText.Length - 1);
                }
                double xInt = Convert.ToDouble(xText);
                double yInt = Convert.ToDouble(yText);
                if (xInt == yInt)
                {
                    compareResult = 0;
                }
                else if (xInt < yInt)
                {
                    compareResult = -1;
                }
                else
                {
                    compareResult = 1;
                }
            }
            else
            {
                DateTime d1 = Convert.ToDateTime(xText);
                DateTime d2 = Convert.ToDateTime(yText);
                if (d1 == d2)
                {
                    compareResult = 0;
                }
                else if (d1 < d2)
                {
                    compareResult = -1;
                }
                else
                {
                    compareResult = 1;
                }
            }
            // 根据上面的比较结果返回正确的比较结果
            if (OrderOfSort == System.Windows.Forms.SortOrder.Ascending)
            {
                // 因为是正序排序，所以直接返回结果
                return compareResult;
            }
            else if (OrderOfSort == System.Windows.Forms.SortOrder.Descending)
            {
                // 如果是反序排序，所以要取负值再返回
                return (-compareResult);
            }
            else
            {
                // 如果相等返回0
                return 0;
            }
        }
        /// <summary>
        /// 获取或设置按照哪一列排序.
        /// </summary>
        public int SortColumn
        {
            set
            {
                ColumnToSort = value;
            }
            get
            {
                return ColumnToSort;
            }
        }

        public OrderType SortType { get; set; }
        /// <summary>
        /// 获取或设置排序方式.
        /// </summary>
        public System.Windows.Forms.SortOrder Order
        {
            set
            {
                OrderOfSort = value;
            }
            get
            {
                return OrderOfSort;
            }
        }
    }
}