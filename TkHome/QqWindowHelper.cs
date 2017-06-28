using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accessibility;

namespace WindowsFormsApplication1
{
    class QqWindowHelper
    {
        IntPtr _QqWindowHandle;
        IAccessible _inputBox;

        public QqWindowHelper(IntPtr windowHandle)
        {
            _QqWindowHandle = windowHandle;

            GetAccessibleObjects(_QqWindowHandle, out _inputBox);
        }

        /// <summary>
        /// 返回消息框内容
        /// </summary>
        /// <returns></returns>
        public string GetContent()
        {
            if(_inputBox != null)
            {
                string value = (string)_inputBox.get_accValue(Win32.CHILDID_SELF);
                return value;
            }
            return "";
        }

        private IAccessible[] GetAccessibleChildren(IAccessible paccContainer)
        {
            IAccessible[] rgvarChildren = new IAccessible[paccContainer.accChildCount];
            int pcObtained;
            Win32.AccessibleChildren(paccContainer, 0, paccContainer.accChildCount, rgvarChildren, out pcObtained);
            return rgvarChildren;
        }
        //按层级找到对象
        public IAccessible GetAccessibleChild(IAccessible paccContainer, int[] array)
        {
            if (array.Length > 0)
            {
                IAccessible result = GetAccessibleChildren(paccContainer)[array[0]];

                int[] array_1 = new int[array.Length - 1];
                for (int i = 0; i < array.Length - 1; i++)
                {
                    array_1[i] = array[i + 1];
                }
                return GetAccessibleChild(result, array_1);
            }
            else
            {
                return paccContainer;
            }
        }

        public IAccessible GetMsgChild(IAccessible paccContainer)
        {
            try
            {
                string name = (string)paccContainer.get_accName(Win32.CHILDID_SELF);
                if(name == "消息")
                {
                    return paccContainer;
                }
                else
                {
                    if(paccContainer.accChildCount > 0)
                    {
                        object[] rgvarChildren = new object[paccContainer.accChildCount];
                        int pcObtained;
                        Win32.AccessibleChildren(paccContainer, 0, paccContainer.accChildCount, rgvarChildren, out pcObtained);
                        foreach(object child in rgvarChildren)
                        {
                            if (child is IAccessible)
                            {
                                IAccessible ret = GetMsgChild((IAccessible)child);
                                if (ret != null)
                                {
                                    return ret;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

            }

            return null;
        }

        private void GetAccessibleObjects(System.IntPtr imWindowHwnd, out IAccessible inputBox)
        {
            Guid guidCOM = new Guid(0x618736E0, 0x3C3D, 0x11CF, 0x81, 0xC, 0x0, 0xAA, 0x0, 0x38, 0x9B, 0x71);
            Accessibility.IAccessible IACurrent = null;

            Win32.AccessibleObjectFromWindow(imWindowHwnd, (int)Win32.OBJID_CLIENT, ref guidCOM, ref IACurrent);
            IACurrent = (IAccessible)IACurrent.accParent;
            inputBox = null;
            inputBox = GetMsgChild(IACurrent);
        }
    }
}
