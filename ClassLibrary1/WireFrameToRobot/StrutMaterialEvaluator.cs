using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DesignScript.Runtime;


namespace WireFrameToRobot
{
    /// <summary>
    /// this class holds methods for evaluating struts based on their material assignement
    /// </summary>
    public static class StrutMaterialEvaluator
    {
        /// <summary>
        /// this method evaluates a set of struts and returns lists 
        /// containing the struts that passed and those that failed the evaluation
        /// </summary>
        /// <param name="strutSolution"></param>
        /// <returns></returns>
        [MultiReturn(new string[]{"passed", "failed"})]
        public static Dictionary<string,object> EvaluateStruts(List<Strut> strutSolution)
        {

            var outputDict = new Dictionary<string, object>();
           

            //TODO this method should probably accept a set of functions or expose some variables that define
            // the requirments for passing and failure.

            // for now we just check something simple - CrossSection * sqrt(length) * materialIndex > some static load number
            // if this equation is true then the strut passes else fails, we can consider the fittness of the solution to be
            // passing.num/total.num this is just a placeholder and will probably need to accept a reactObject with some forces
            // or we can add these to the strut class - and they will be updated when react values are recalculated.
            
            //we just need to add more equations to this list of functions and we'll filter them, we can also perform a similar
            //calculation in Dynamo directly, or these can be exposed as regular methods and Dynamo will convert them to
            //functions automatically if they left unhooked
            var equation1 = new Func<Strut, bool>(
                (Strut strut) => 
                {
                    var pass = (strut.Diameter * (Math.Sqrt(strut.LineRepresentation.Length)) * (int)strut.Material ) > 100  ;
                  
                    return pass;
                }
                    );
            var tests = new List<Func<Strut,bool>>();
            tests.Add(equation1);

            var passed = strutSolution.Where(strut => tests.All(test => test.Invoke(strut) == true)).ToList();
            var failed = strutSolution.Where(strut => tests.Any(test => test.Invoke(strut) == false)).ToList();

            outputDict.Add("passed", passed);
            outputDict.Add("failed", failed);

            return outputDict;
        }
        
    }
}
