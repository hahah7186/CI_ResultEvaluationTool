using CI_ResultEvaluationTool.JUNIT_XML;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace CI_ResultEvaluationTool.Evaluation
{
    public class EvaluationProcess
    {
        public enum ResultFileType
        {
            ReSharper = 0,
            ProjectCheck = 1,
            UnittestTool = 2,
            Others = 4
        };

        private ResultFileType resultFileType;

        private string resultFilePath;

        private string evaluationFlePath;

        private TestEvaluationRules testEvaluationRules;

        private Testsuites testsuites;

        private XmlDocument JUNIT_XML_Doc;

        public EvaluationProcess(string resultFilePath, string evaluationFlePath)
        {
            this.resultFilePath = resultFilePath;
            this.evaluationFlePath = evaluationFlePath;
            this.resultFileType = CheckFileType(resultFilePath);
            this.testEvaluationRules = new TestEvaluationRules(evaluationFlePath);
            ParseResultFile();
        }

        private void ParseResultFile()
        {
            int errorCount = 0;
            int testCount = 0;

            testsuites = new Testsuites();
            testsuites.Disabled = "0";
            testsuites.Errors = "10";
            testsuites.Failures = "0";
            testsuites.Name = "";
            testsuites.Tests = "20";
            testsuites.Time = "";

            Testsuite testsuite = new Testsuite();

            XmlDocument doc = new XmlDocument();
            doc.Load(@resultFilePath);
            //1.1 Get GlobalRuleString
            XPathNavigator GlobalRule_XPathNav = doc.CreateNavigator();
            XPathNodeIterator GlobalRuleEntryIterator = GlobalRule_XPathNav.Select(testEvaluationRules.GlobalTestsStatus.GlobalRuleString);

            string TotalResult = "";

            while (GlobalRuleEntryIterator.MoveNext()) {
                XPathNavigator itemNav = GlobalRuleEntryIterator.Current;
                TotalResult = itemNav.Value;
            }
            //1.2 Get GlobalTimeStamp
            XPathNavigator GlobalTimeStamp_XPathNav = doc.CreateNavigator();
            XPathNodeIterator GlobalTimeStampEntryIterator = GlobalTimeStamp_XPathNav.Select(testEvaluationRules.GlobalTestsStatus.GlobalTimeStamp);

            string GlobalTimeStamp = "";

            while (GlobalTimeStampEntryIterator.MoveNext())
            {
                XPathNavigator itemNav = GlobalTimeStampEntryIterator.Current;
                GlobalTimeStamp = itemNav.Value;
            }

            //2.1 Get SingleResultTestString of all the tests
            List<Testcase> testTestcases = new List<Testcase>();
            XPathNavigator SingleRuleTest_XPathNav = doc.CreateNavigator();
            XPathNodeIterator SingleRuleTestEntryIterator = SingleRuleTest_XPathNav.Select(testEvaluationRules.SingleTestStatus.SingleResultTestString);

            while (SingleRuleTestEntryIterator.MoveNext())
            {
                Testcase testcase = new Testcase();

                XPathNavigator itemNav = SingleRuleTestEntryIterator.Current;
                string name = itemNav.GetAttribute("Name", "");
                string classname = itemNav.GetAttribute("xsi:type", "");

                testcase.Name = name;
                testcase.Classname = classname;
                testcase.Assertions = "1";
                testcase.Status = "";
                testcase.Time = "";
                testCount++;
                testTestcases.Add(testcase);
            }

            //2.2 Get SingleResultTestString of all the errors
            List<Testcase> errorTestcases = new List<Testcase>();

            XPathNavigator SingleRule_XPathNav = doc.CreateNavigator();
            XPathNodeIterator SingleRuleEntryIterator = SingleRule_XPathNav.Select(testEvaluationRules.SingleTestStatus.SingleResultRuleString);

            while (SingleRuleEntryIterator.MoveNext())
            {
                Testcase testcase = new Testcase();

                XPathNavigator itemNav = SingleRuleEntryIterator.Current;
                string name = itemNav.GetAttribute("Name", "");
                string classname = itemNav.GetAttribute("xsi:type", "");

                testcase.Name = name;
                testcase.Classname = classname;
                testcase.Assertions = "1";
                testcase.Status = "";
                testcase.Time = "";

                Failure failure = new Failure();
                
                string message = itemNav.SelectSingleNode(testEvaluationRules.SingleTestStatus.SingleResultMessageString) != null ? itemNav.SelectSingleNode(testEvaluationRules.SingleTestStatus.SingleResultMessageString).Value : "";

                failure.Message = message;
                failure.Type = "ValueMismatch";

                testcase.SetFailure(failure);
                errorCount++;


                errorTestcases.Add(testcase);
            }

            string fileName = System.IO.Path.GetFileNameWithoutExtension(@resultFilePath);
            testsuite.Name = fileName;
            testsuite.Tests = testCount.ToString();
            testsuite.Disabled = "0";
            testsuite.Errors = errorCount.ToString();
            testsuite.Failures = "0";
            testsuite.Id = "0";
            testsuite.Skipped = "0";
            testsuite.Time = "";
            testsuite.Timestamp = GlobalTimeStamp;
            testsuite.Testcases = errorTestcases;

            testsuites.TestsuiteList = new List<Testsuite>();
            testsuites.TestsuiteList.Add(testsuite);
            testsuites.Errors = errorCount.ToString();
            testsuites.Tests = testCount.ToString();
            GenerateJUNITXML(fileName);
        }

        private void GenerateJUNITXML(string filename)
        {
            XmlDocument document = new XmlDocument();//创建XmlDocument对象

            XmlDeclaration declaration = document.CreateXmlDeclaration("1.0", "UTF-8", "");//xml文档的声明部分
            document.AppendChild(declaration);

            XmlElement testsuiteRootNode = document.CreateElement("", "testsuites", "");
            testsuiteRootNode.SetAttribute("disabled", testsuites.Disabled);
            testsuiteRootNode.SetAttribute("errors", testsuites.Errors);
            testsuiteRootNode.SetAttribute("failures", testsuites.Failures);
            testsuiteRootNode.SetAttribute("name", testsuites.Name);
            testsuiteRootNode.SetAttribute("tests", testsuites.Tests);
            testsuiteRootNode.SetAttribute("time", testsuites.Time);
            document.AppendChild(testsuiteRootNode);

            foreach (Testsuite testsuite in testsuites.TestsuiteList) {
                XmlElement testsuiteNode = document.CreateElement("testsuite");
                testsuiteNode.SetAttribute("name",testsuite.Name);
                testsuiteNode.SetAttribute("tests", testsuite.Tests);
                testsuiteNode.SetAttribute("disabled", "0");
                testsuiteNode.SetAttribute("errors", "10");
                testsuiteNode.SetAttribute("failures", "0");
                testsuiteNode.SetAttribute("id", "0");
                testsuiteNode.SetAttribute("skipped", "0");
                testsuiteNode.SetAttribute("time", "");
                testsuiteNode.SetAttribute("timestamp", testsuite.Timestamp);

                foreach (Testcase testcase in testsuite.Testcases) {
                    XmlElement testcaseNode = document.CreateElement("testcase");
                    testcaseNode.SetAttribute("name", testcase.Name);
                    testcaseNode.SetAttribute("assertions", "1");
                    testcaseNode.SetAttribute("classname", testcase.Classname);
                    testcaseNode.SetAttribute("status", "");
                    testcaseNode.SetAttribute("time", "");

                    XmlElement failureNode = document.CreateElement("failure");
                    failureNode.SetAttribute("message", testcase.GetFailure().Message);
                    failureNode.SetAttribute("type", testcase.GetFailure().Type);
                    testcaseNode.AppendChild(failureNode);

                    testsuiteNode.AppendChild(testcaseNode);
                }

                testsuiteRootNode.AppendChild(testsuiteNode);
            }

            //XmlElement zwerks = document.CreateElement("ZWERKS");
            //zwerks.InnerText = "ZFM1";
            //root.AppendChild(zwerks);

            //XmlElement tab1 = document.CreateElement("TAB1");
            //root.AppendChild(tab1);

            //XmlElement zno = document.CreateElement("ZNO");
            //zno.InnerText = "13022101";
            //tab1.AppendChild(zno);

            //XmlElement zorder = document.CreateElement("ZORDER");
            //zorder.InnerText = "2013238955";
            //tab1.AppendChild(zorder);

            //XmlElement zweight = document.CreateElement("ZWEIGHT");
            //zweight.InnerText = "4140";
            //tab1.AppendChild(zweight);

            //XmlElement tab2 = document.CreateElement("TAB1");
            //root.AppendChild(tab2);

            //XmlElement zno2 = document.CreateElement("ZNO");
            //zno2.InnerText = "13022101";
            //tab2.AppendChild(zno2);

            //XmlElement zorder2 = document.CreateElement("ZORDER");
            //zorder2.InnerText = "2013238955";
            //tab2.AppendChild(zorder2);

            //XmlElement zweight2 = document.CreateElement("ZWEIGHT");
            //zweight2.InnerText = "4140";
            //tab2.AppendChild(zweight2);

            document.Save("../../../Output/" + filename + "_junit.xml");//将生成好的xml保存到test.xml文件中

        }

        private ResultFileType CheckFileType(string filePath)
        {
            //Get file name from full file path.
            string fileName = System.IO.Path.GetFileName(filePath);

            if (fileName.ToLower().Contains("ReSharper".ToLower()))
            {
                return ResultFileType.ReSharper;
            }
            else if (fileName.ToLower().Contains("ProjectCheck".ToLower()))
            {
                return ResultFileType.ProjectCheck;
            }
            else if (fileName.ToLower().Contains("UnittestTool".ToLower()))
            {
                return ResultFileType.UnittestTool;
            }
            else
            {
                return ResultFileType.Others;
            }
        }
    }
}
