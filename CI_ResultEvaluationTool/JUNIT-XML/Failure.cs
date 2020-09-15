using System;
using System.Collections.Generic;
using System.Text;

namespace CI_ResultEvaluationTool.JUNIT_XML
{
    class Failure
    {
        private string message;
        private string type;

        public string Message { get => message; set => message = value; }
        public string Type { get => type; set => type = value; }
    }
}
