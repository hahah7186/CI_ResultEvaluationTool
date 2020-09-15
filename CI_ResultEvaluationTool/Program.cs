using System;
using System.Collections.Generic;
using CI_ResultEvaluationTool.Evaluation;

namespace CI_ResultEvaluationTool
{
    class Program
    {
        static int Main(string[] args)
        {
            //Get all the Result files and Evaluation files.
            string[] filePaths = args;

            List<EvaluationProcess> evaluationList = new List<EvaluationProcess>();

            //Determine if the args appears in pairs. 
            //Otherwise, the input params are not valid so that program will return -1 to Jenkins.
            if (filePaths.Length % 2 != 0)
            {
                return -1;
            }
            else
            {//If in pairs, means that result file corresponds to evaluation file.
                for (int i = 0; i < filePaths.Length; i += 2)
                {

                    //For both paired files, new create an Object Evaluation
                    EvaluationProcess evaluation = new EvaluationProcess(filePaths[i], filePaths[i + 1]);

                    evaluationList.Add(evaluation);
                }
            }

            return 0;
        }
    }
}
