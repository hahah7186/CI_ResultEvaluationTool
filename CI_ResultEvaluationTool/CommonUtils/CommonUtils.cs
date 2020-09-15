using System;
using System.Collections.Generic;
using System.Text;

namespace CI_ResultEvaluationTool.CommonUtils
{
    public static class CommonUtils
    {
        public static string ReplaceAndNewlines(string inputStr) {
            return inputStr.Replace(System.Environment.NewLine, string.Empty).Replace("\t", "").Trim();
        }
    }
}
