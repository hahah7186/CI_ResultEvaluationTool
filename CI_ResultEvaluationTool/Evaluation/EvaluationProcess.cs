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
        //Initialize the parse result equals -1.
        public int parseResult = -1;

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
            int failureCount = 0;
            int skipCount = 0;
            int successCount = 0;
            int testCount = 0;

            List<Testcase> testTestcases = new List<Testcase>();
            List<Testcase> failureTestcases = new List<Testcase>();
            List<Testcase> errorTestcases = new List<Testcase>();
            List<Testcase> successTestcases = new List<Testcase>();
            List<Testcase> skipTestcases = new List<Testcase>();

            //Initialize Testsuites label.
            testsuites = new Testsuites();
            testsuites.Disabled = "0";
            testsuites.Errors = "10";
            testsuites.Failures = "0";
            testsuites.Name = "";
            testsuites.Tests = "20";
            testsuites.Time = "";
            //New create testsuite label.
            Testsuite testsuite = new Testsuite();
            //Read result file path from input params.
            XmlDocument doc = new XmlDocument();
            doc.Load(@resultFilePath);
            //1.1 Get GlobalRuleString
            //Get global result from result file.
            string GlobalResult = "";

            if (testEvaluationRules.GlobalTestsStatus.GlobalRuleString != "") {
                XPathNavigator GlobalRule_XPathNav = doc.CreateNavigator();
                XPathNodeIterator GlobalRuleEntryIterator = GlobalRule_XPathNav.Select(testEvaluationRules.GlobalTestsStatus.GlobalRuleString);

                while (GlobalRuleEntryIterator.MoveNext()) {
                    XPathNavigator itemNav = GlobalRuleEntryIterator.Current;
                    GlobalResult = itemNav.Value;
                }
            }

            //1.2 Get GlobalTimeStamp
            //Get global time stamp from result file.
            string GlobalTimeStamp = "";

            if (testEvaluationRules.GlobalTestsStatus.GlobalTimeStamp != "") {
                XPathNavigator GlobalTimeStamp_XPathNav = doc.CreateNavigator();
                XPathNodeIterator GlobalTimeStampEntryIterator = GlobalTimeStamp_XPathNav.Select(testEvaluationRules.GlobalTestsStatus.GlobalTimeStamp);
            
                while (GlobalTimeStampEntryIterator.MoveNext())
                {
                    XPathNavigator itemNav = GlobalTimeStampEntryIterator.Current;
                    GlobalTimeStamp = itemNav.Value;
                }
            }


            //2.1 Get SingleResultTestString of all the TESTS
            if (testEvaluationRules.SingleTestStatus.SingleResultTestRuleString != "") {
                XPathNavigator SingleRuleTest_XPathNav = doc.CreateNavigator();
                XPathNodeIterator SingleRuleTestEntryIterator = SingleRuleTest_XPathNav.Select(testEvaluationRules.SingleTestStatus.SingleResultTestRuleString);

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
            }


            //2.2 Get SingleResultErrorRuleString of all the ERRORS.
            if (testEvaluationRules.SingleTestStatus.SingleResultErrorRuleString != "") {
                XPathNavigator SingleRule_XPathNav = doc.CreateNavigator();
                XPathNodeIterator SingleRuleEntryIterator = SingleRule_XPathNav.Select(testEvaluationRules.SingleTestStatus.SingleResultErrorRuleString);

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

                    string message = itemNav.SelectSingleNode(testEvaluationRules.SingleTestStatus.SingleResultErrorMessageString) != null ? itemNav.SelectSingleNode(testEvaluationRules.SingleTestStatus.SingleResultErrorMessageString).Value : itemNav.GetAttribute(testEvaluationRules.SingleTestStatus.SingleResultErrorMessageString, "");

                    failure.Message = message;
                    failure.Type = "ValueMismatch";

                    testcase.SetFailure(failure);
                    errorCount++;


                    errorTestcases.Add(testcase);
                }
            }

            //2.3 Get SingleResultFailureRuleString of all the FAILURE.
            if (testEvaluationRules.SingleTestStatus.SingleResultFailureRuleString != "")
            {
                XPathNavigator SingleRule_XPathNav = doc.CreateNavigator();
                XPathNodeIterator SingleRuleEntryIterator = SingleRule_XPathNav.Select(testEvaluationRules.SingleTestStatus.SingleResultFailureRuleString);

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

                    string message = itemNav.SelectSingleNode(testEvaluationRules.SingleTestStatus.SingleResultFailureMessageString) != null ? itemNav.SelectSingleNode(testEvaluationRules.SingleTestStatus.SingleResultFailureMessageString).Value : itemNav.GetAttribute(testEvaluationRules.SingleTestStatus.SingleResultFailureMessageString,"");

                    failure.Message = message;
                    failure.Type = "ValueMismatch";

                    testcase.SetFailure(failure);
                    failureCount++;


                    failureTestcases.Add(testcase);
                }
            }

            //2.4 Get SingleResultSuccessRuleString of all the SUCCESS.
            if (testEvaluationRules.SingleTestStatus.SingleResultSuccessRuleString != "")
            {
                XPathNavigator SingleRule_XPathNav = doc.CreateNavigator();
                XPathNodeIterator SingleRuleEntryIterator = SingleRule_XPathNav.Select(testEvaluationRules.SingleTestStatus.SingleResultSuccessRuleString);

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

                    string message = itemNav.SelectSingleNode(testEvaluationRules.SingleTestStatus.SingleResultSuccessRuleString) != null ? itemNav.SelectSingleNode(testEvaluationRules.SingleTestStatus.SingleResultSuccessRuleString).Value : itemNav.GetAttribute(testEvaluationRules.SingleTestStatus.SingleResultSuccessRuleString, "");

                    failure.Message = message;
                    failure.Type = "ValueMismatch";

                    testcase.SetFailure(failure);
                    successCount++;


                    successTestcases.Add(testcase);
                }
            }


            //2.4 Get SingleResultSkipedRuleString of all the SKIP.
            if (testEvaluationRules.SingleTestStatus.SingleResultSkippedRuleString != "")
            {
                XPathNavigator SingleRule_XPathNav = doc.CreateNavigator();
                XPathNodeIterator SingleRuleEntryIterator = SingleRule_XPathNav.Select(testEvaluationRules.SingleTestStatus.SingleResultSkippedRuleString);

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

                    string message = itemNav.SelectSingleNode(testEvaluationRules.SingleTestStatus.SingleResultSkippedRuleString) != null ? itemNav.SelectSingleNode(testEvaluationRules.SingleTestStatus.SingleResultSkippedRuleString).Value : itemNav.GetAttribute(testEvaluationRules.SingleTestStatus.SingleResultSkippedRuleString, "");

                    failure.Message = message;
                    failure.Type = "ValueMismatch";

                    testcase.SetFailure(failure);
                    skipCount++;


                    skipTestcases.Add(testcase);
                }
            }

            List<Testcase> totalTestcases = new List<Testcase>();
            //if (testTestcases.Count > 0) {
            //    totalTestcases.AddRange(testTestcases);
            //}
            if (errorTestcases.Count > 0) {
                totalTestcases.AddRange(errorTestcases);
            }
            if (successTestcases.Count > 0) {
                totalTestcases.AddRange(successTestcases);
            }
            if (skipTestcases.Count > 0) {
                totalTestcases.AddRange(skipTestcases);
            }
            if (failureTestcases.Count > 0) {
                totalTestcases.AddRange(failureTestcases);
            }


            string fileName = System.IO.Path.GetFileNameWithoutExtension(@resultFilePath);
            testsuite.Name = fileName;
            testsuite.Tests = testCount.ToString();
            testsuite.Disabled = "0";
            testsuite.Errors = errorCount.ToString();
            testsuite.Failures = failureCount.ToString();
            testsuite.Id = "0";
            testsuite.Skipped = skipCount.ToString();
            testsuite.Time = "";
            testsuite.Timestamp = GlobalTimeStamp;
            testsuite.Testcases = totalTestcases;

            testsuites.TestsuiteList = new List<Testsuite>();
            testsuites.TestsuiteList.Add(testsuite);
            testsuites.Errors = errorCount.ToString();
            testsuites.Tests = testCount.ToString();
            testsuites.Failures = failureCount.ToString();
            GenerateJUNITXML(fileName);

            if (GlobalResult == "Failure") {
                this.parseResult = -1;
            } else if (GlobalResult == "Success") {
                this.parseResult = 0;
            }

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
                testsuiteNode.SetAttribute("disabled", testsuite.Disabled);
                testsuiteNode.SetAttribute("errors", testsuite.Errors);
                testsuiteNode.SetAttribute("failures", testsuite.Failures);
                testsuiteNode.SetAttribute("id", "0");
                testsuiteNode.SetAttribute("skipped", testsuite.Skipped);
                testsuiteNode.SetAttribute("time", "");
                testsuiteNode.SetAttribute("timestamp", testsuite.Timestamp);

                foreach (Testcase testcase in testsuite.Testcases) {
                    XmlElement testcaseNode = document.CreateElement("testcase");
                    testcaseNode.SetAttribute("name", testcase.Name);
                    testcaseNode.SetAttribute("assertions", "1");
                    testcaseNode.SetAttribute("classname", testcase.Classname);
                    testcaseNode.SetAttribute("status", "");
                    testcaseNode.SetAttribute("time", "");
                    if (testcase.GetFailure().Message != "") {
                        XmlElement failureNode = document.CreateElement("failure");
                        failureNode.SetAttribute("message", testcase.GetFailure().Message);
                        failureNode.SetAttribute("type", testcase.GetFailure().Type);
                        testcaseNode.AppendChild(failureNode);
                    }
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
