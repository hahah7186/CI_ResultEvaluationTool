using System;
using System.Collections.Generic;
using System.Text;

namespace CI_ResultEvaluationTool.JUNIT_XML
{
    class Testcase
    {
        private string name;
        private string assertions;
        private string classname;
        private string status;
        private string time;
        private Failure failure;

        public string Name { get => name; set => name = value; }
        public string Assertions { get => assertions; set => assertions = value; }
        public string Classname { get => classname; set => classname = value; }
        public string Status { get => status; set => status = value; }
        public string Time { get => time; set => time = value; }

        public Failure GetFailure()
        {
            return failure;
        }

        public void SetFailure(Failure value)
        {
            failure = value;
        }
    }
}
