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
            //1.Read evaluation.xml file.
            XmlDocument doc = new XmlDocument();
            doc.Load(@evaFilePath);

            //2.Create the global test status class.

            //2.1 To get the global rule string from evaluation.xml.
            string globalRuleString = doc.SelectSingleNode("//GlobalTestsStatus[1]/Failure[1]").InnerText;
            //format string.
            globalRuleString = CommonUtils.CommonUtils.ReplaceAndNewlines(globalRuleString);
            //2.2 To get the global timestamp rule string from evaluation.xml.
            string globalTimeStamp = doc.SelectSingleNode("//GlobalTestsStatus[1]/TimeStamp[1]").InnerText;
            //Create the global test status class.
            this.globalTestsStatus = new GlobalTestsStatus(globalRuleString, globalTimeStamp);

            //3. Create the single test status class.

            //3.1 To get the single result FAILURE rule string from evaluation.xml.
            string singleResultFailureRuleString = doc.SelectSingleNode("//SingleTestStatus[1]/Failures[1]/Rule[1]").InnerText;
            singleResultFailureRuleString = CommonUtils.CommonUtils.ReplaceAndNewlines(singleResultFailureRuleString);

            string singleResultFailureMessageString = doc.SelectSingleNode("//SingleTestStatus[1]/Failures[1]/Message[1]").InnerText;
            singleResultFailureMessageString = CommonUtils.CommonUtils.ReplaceAndNewlines(singleResultFailureMessageString);

            //3.2 To get the single result ERROR rule string & message string from evaluation.xml.
            string singleResultErrorRuleString = doc.SelectSingleNode("//SingleTestStatus[1]/Errors[1]/Rule[1]").InnerText;
            singleResultErrorRuleString = CommonUtils.CommonUtils.ReplaceAndNewlines(singleResultErrorRuleString);

            string singleResultErrorMessageString = doc.SelectSingleNode("//SingleTestStatus[1]/Errors[1]/Message[1]").InnerText;
            singleResultErrorMessageString = CommonUtils.CommonUtils.ReplaceAndNewlines(singleResultErrorMessageString);

            //3.3 To get the single result TEST rule string & message string from evaluation.xml.
            string singleResultTestRuleString = doc.SelectSingleNode("//SingleTestStatus[1]/Tests[1]/Rule[1]").InnerText;
            singleResultTestRuleString = CommonUtils.CommonUtils.ReplaceAndNewlines(singleResultTestRuleString);

            //3.4 To get the single result SUCCESS rule string & message string from evaluation.xml.
            string singleResultSuccessRuleString = doc.SelectSingleNode("//SingleTestStatus[1]/Successes[1]/Rule[1]").InnerText;
            singleResultSuccessRuleString = CommonUtils.CommonUtils.ReplaceAndNewlines(singleResultSuccessRuleString);

            //3.5 To get the single result SKIP rule string & message string from evaluation.xml.
            string singleResultSkippedRuleString = doc.SelectSingleNode("//SingleTestStatus[1]/Skipped[1]/Rule[1]").InnerText;
            singleResultSkippedRuleString = CommonUtils.CommonUtils.ReplaceAndNewlines(singleResultSkippedRuleString);

            //Create the single test status class
            this.singleTestStatus = new SingleTestStatus(singleResultTestRuleString, singleResultErrorRuleString, singleResultErrorMessageString, singleResultFailureRuleString, singleResultFailureMessageString, singleResultSuccessRuleString, singleResultSkippedRuleString);
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
        private string singleResultTestRuleString;

        private string singleResultErrorRuleString;
        private string singleResultErrorMessageString;

        private string singleResultFailureRuleString;
        private string singleResultFailureMessageString;

        private string singleResultSuccessRuleString;
        //private string singleResultSuccessMessageString;

        private string singleResultSkippedRuleString;
        //private string singleResultSkippedMessageString;

        public SingleTestStatus(string singleResultTestRuleString, string singleResultErrorRuleString, string singleResultErrorMessageString, string singleResultFailureRuleString, string singleResultFailureMessageString, string singleResultSuccessRuleString, string singleResultSkippedRuleString )
        {
            this.singleResultTestRuleString = singleResultTestRuleString;
            this.singleResultErrorRuleString = singleResultErrorRuleString;
            this.singleResultErrorMessageString = singleResultErrorMessageString;
            this.singleResultFailureRuleString = singleResultFailureRuleString;
            this.singleResultFailureMessageString = singleResultFailureMessageString;
            this.singleResultSuccessRuleString = singleResultSuccessRuleString;
            //this.singleResultSuccessMessageString = singleResultSuccessMessageString;
            this.singleResultSkippedRuleString = singleResultSkippedRuleString;
            //this.singleResultSkippedMessageString = singleResultSkippedMessageString;
        }

        public string SingleResultTestRuleString { get => singleResultTestRuleString; }
        public string SingleResultErrorRuleString { get => singleResultErrorRuleString; }
        public string SingleResultErrorMessageString { get => singleResultErrorMessageString; }
        public string SingleResultFailureRuleString { get => singleResultFailureRuleString; }
        public string SingleResultFailureMessageString { get => singleResultFailureMessageString; }
        public string SingleResultSuccessRuleString { get => singleResultSuccessRuleString; }
        //public string SingleResultSuccessMessageString { get => singleResultSuccessMessageString; }
        public string SingleResultSkippedRuleString { get => singleResultSkippedRuleString; }
        //public string SingleResultSkippedMessageString { get => singleResultSkippedMessageString; }
    }
}
