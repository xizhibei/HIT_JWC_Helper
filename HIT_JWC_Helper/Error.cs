using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace HIT_JWC_Helper
{
    class Error
    {
        static public void RecordLog(Exception e,string msg)
        {
            string errormsg = "[" + DateTime.Now.ToString() + "]\n" + e.Message + "\n" + e.StackTrace;
            File.AppendAllText("error.log", errormsg);
            MessageBox.Show(msg, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
