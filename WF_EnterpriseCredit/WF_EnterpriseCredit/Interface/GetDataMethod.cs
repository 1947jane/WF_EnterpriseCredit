using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace WF_EnterpriseCredit.Interface
{
  public  interface  GetDataMethod
    {
      string GetDataByGet(string strUrl, ref CookieContainer cookie, RequestConent RequestConent);

      string GetDataByPost(string strUrl, ref CookieContainer cookie, RequestConent RequestConent, string strpostdata);
        
    }
  public class RequestConent
  {
      public string StrHost { get; set; }
      public string StrAccept { get; set; }
      public string StrReferer { get; set; }
      public string ContentType { get; set; }
      public string UserAgent { get; set; }
      public int Timeout { get; set; }
  }
}
