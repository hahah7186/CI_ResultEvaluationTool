using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using CI_ResultEvaluationTool.CommonUtils;

namespace CI_ResultEvaluationTool.Evaluation
{
    class TestEvaluationRules
    {
        //Path of evaluation file
        private string evaFilePath;
        private GlobalTestsStatus globalTestsStatus;
        private SingleTestStatus singleTestStatus;

        public TestEvaluationRules(string evaFilePath)
        {
            this.evaFilePath = evaFilePath;
            this.parseXmlFile(evaFilePath);
        }

        internal GlobalTestsStatus GlobalTestsStatus { get => globalTestsStatus; }
        internal SingleTestStatus SingleTestStatus { get => singleTestStatus; }

        private void parseXmlFile(string evaFilePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@evaFilePath);

            string globalRuleString = doc.SelectSingleNode("//GlobalTestsStatus[1]/Error[1]").InnerText;
            globalRuleString = CommonUtils.CommonUtils.ReplaceAndNewlines(globalRuleString);

            string globalTimeStamp = doc.SelectSingleNode("//GlobalTestsStatus[1]/TimeStamp[1]").InnerText;

            this.globalTestsStatus = new GlobalTestsStatus(globalRuleString, globalTimeStamp);

            string singleResultErrorRuleString = doc.SelectSingleNode("//SingleTestStatus[1]/Error[1]/ErrorRule[1]").InnerText;
            singleResultErrorRuleString = CommonUtils.CommonUtils.ReplaceAndNewlines(singleResultErrorRuleString);

            string singleResultErrorMessageString = doc.SelectSingleNode("//SingleTestStatus[1]/Error[1]/ErrorMessage[1]").InnerText;
            singleResultErrorMessageString = CommonUtils.CommonUtils.ReplaceAndNewlines(singleResultErrorMessageString);

            string singleResultTestString = doc.SelectSingleNode("//SingleTestStatus[1]/Test[1]").InnerText;

            this.singleTestStatus = new SingleTestStatus(singleResultErrorRuleString, singleResultErrorMessageString, singleResultTestString);
        }
    }

    class GlobalTestsStatus
    {
        private string globalRuleString;
        private string globalTimeStamp;

        public GlobalTestsStatus(string globalRuleString, string globalTimeStamp)
        {
            this.globalRuleString = globalRuleString;
            this.globalTimeStamp = globalTimeStamp;
        }

        public string GlobalRuleString { get => globalRuleString;  }
        public string GlobalTimeStamp { get => globalTimeStamp;  }
    }

    class SingleTestStatus
    {
        private string singleResultErrorRuleString;

        private string singleResultErrorMessageString;

        private string singleResultTestString;

        public SingleTestStatus(string singleResultErrorRuleString, string singleResultErrorMessageString, string singleResultTestString)
        {
            this.singleResultErrorRuleString = singleResultErrorRuleString;
            this.singleResultErrorMessageString = singleResultErrorMessageString;
            this.singleResultTestString = singleResultTestString;
        }

        public string SingleResultRuleString { get => singleResultErrorRuleString;  }
        public string SingleResultMessageString { get => singleResultErrorMessageString;  }
        public string SingleResultTestString { get => singleResultTestString; }
    }
}
