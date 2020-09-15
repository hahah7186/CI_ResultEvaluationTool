using System;
using System.Collections.Generic;
using System.Text;

namespace CI_ResultEvaluationTool.JUNIT_XML
{
    class Testsuites
    {
        private string name;
        private string disabled;
        private string tests;
        private string time;
        private string errors;
        private string failures;
        private List<Testsuite> testsuites;

        public string Name { get => name; set => name = value; }
        public string Tests { get => tests; set => tests = value; }
        public string Time { get => time; set => time = value; }
        public string Errors { get => errors; set => errors = value; }
        public string Failures { get => failures; set => failures = value; }
        public string Disabled { get => disabled; set => disabled = value; }
        public List<Testsuite> TestsuiteList { get => testsuites; set => testsuites = value; }
    }
}
