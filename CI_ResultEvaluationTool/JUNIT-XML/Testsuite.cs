using System;
using System.Collections.Generic;
using System.Text;

namespace CI_ResultEvaluationTool.JUNIT_XML
{
    class Testsuite
    {
        private string name;
        private string failures;
        private string id;
        private string skipped;
        private string time;
        private string timestamp;
        private string tests;
        private string disabled;
        private string errors;
        private List<Testcase> testcases;

        public string Name { get => name; set => name = value; }
        public string Failures { get => failures; set => failures = value; }
        public string Id { get => id; set => id = value; }
        public string Skipped { get => skipped; set => skipped = value; }
        public string Time { get => time; set => time = value; }
        public string Timestamp { get => timestamp; set => timestamp = value; }
        public string Tests { get => tests; set => tests = value; }
        public string Disabled { get => disabled; set => disabled = value; }
        public string Errors { get => errors; set => errors = value; }
        public List<Testcase> Testcases { get => testcases; set => testcases = value; }
    }
}
