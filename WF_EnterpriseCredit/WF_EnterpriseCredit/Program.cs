﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using WF_EnterpriseCredit.Interface;
using System.Collections;

namespace WF_EnterpriseCredit
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ECFrm());
        }
        
    }
}
