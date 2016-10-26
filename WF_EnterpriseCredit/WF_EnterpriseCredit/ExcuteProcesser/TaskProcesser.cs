using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WF_EnterpriseCredit.ExcuteProcesser
{
    public interface TaskProcesser
    {
        string MainRunning();

        void CollRunning(Queue<string> strkeywords);

        void Prepare(MainTaskProcesser taskProcesserLog);
    }
}
