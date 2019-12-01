using System;
using System.Collections.Generic;
using System.Text;

namespace NugetNavigation
{
    public enum NavigationType
    {
        New,
        Back
    }

    public enum PageMode
    {
        Default,//chuyển page như bình thường
        RemovePage,//remove page sau nó
        Modal,//push kiểu modal
        NotRemovePage//dùng trong custompage

    }
}
