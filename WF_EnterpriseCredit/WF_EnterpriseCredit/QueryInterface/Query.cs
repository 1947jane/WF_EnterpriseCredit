using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WF_EnterpriseCredit.LocalEntity;
using WF_EnterpriseCredit.ExcuteProcesser;

namespace WF_EnterpriseCredit.QueryInterface
{
    public interface Query
    {
        LinkedList<T_Web_TianYanCha_POI> GetJson(MainTaskProcesser taskProcesser, string strkeyword);
    }
}
