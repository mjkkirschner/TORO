using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DesignScript.Runtime;
using System.Reflection;

namespace WireFrameToRobot.StrutUtilities
{

    /// <summary>
    /// this class holds a bunch of static methods that we can use inside Dynamo as evaluator functions
    /// OR they can be converted to delegates on the c# side and used below in a c# evaluator
    /// </summary>
    public static class EvalautionMethods
    {
        static bool equation1(Strut strut)

        {
            var surf = Autodesk.DesignScript.Geometry.Surface.ByPatch(strut.Profile);
            var pass = (surf.Area/ (Math.Sqrt(strut.LineRepresentation.Length)) * ((int)strut.Material)+1) > 100;
            surf.Dispose();
            return pass;
        }
    }
    /// <summary>
    /// this class holds methods for evaluating struts based on their material assignement
    /// </summary>
    /// 
    public static class StrutMaterialEvaluator
    {

        /// <summary>
        /// this method converts the evaluatorMethods to delegates that we can call
        /// </summary>
        public static List<Func<Strut, bool>> EvaluatonDelegates
        {
            get
            {
                return typeof(EvalautionMethods).GetMethods(BindingFlags.Static|BindingFlags.NonPublic|BindingFlags.FlattenHierarchy).Select
                          (x => Delegate.CreateDelegate(typeof(Func<Strut, bool>), x)).Cast<Func<Strut, bool>>().ToList();
            }
        }



        /// <summary>
        /// method evaluates a sets of struts and returns a partitioned list of struts that passed all the tests
        /// and those that did not
        /// </summary>
        /// <param name="strutSolution"></param>
        /// <param name="functionsToTest"></param>
        /// <returns></returns>
        [MultiReturn(new string[] { "passed", "failed" })]
        public static Dictionary<string, object> EvaluateStruts(List<Strut> strutSolution, List<Func<Strut, bool>> functionsToTest)
        {

            var outputDict = new Dictionary<string, object>();

            var passed = strutSolution.Where(strut => functionsToTest.All(test => test.Invoke(strut) == true)).ToList();
            var failed = strutSolution.Where(strut => functionsToTest.Any(test => test.Invoke(strut) == false)).ToList();

            outputDict.Add("passed", passed);
            outputDict.Add("failed", failed);

            return outputDict;
        }


        /// <summary>
        /// create a first solution from a material and struts, all struts are set to this material
        /// and then the solution is evaluated
        /// </summary>
        /// <param name="struts"></param>
        /// <param name="material"></param>
        /// <returns></returns>
        public static StrutSolution GenerateInitialSolutionByStrutsMaterial(List<Strut> struts, Material material)
        {
            //set initial materials
            var newStruts = Strut.ByStrutsAndMaterials(struts, Enumerable.Repeat(material, struts.Count).ToList());

            //evaluate this solution
            var resultDict = EvaluateStruts(newStruts, EvaluatonDelegates);
            var passing = resultDict["passed"] as List<Strut>;
            var failing = resultDict["failed"] as List<Strut>;

            var initialSolution = new StrutSolution(newStruts,passing,failing,passing.Count()/newStruts.Count);
            return initialSolution;
        }

        /// <summary>
        /// this function iterates on a previous solution to produce a new solution
        /// </summary>
        /// <param name="oldSolution"></param>
        /// <returns></returns>
        public static StrutSolution GenerateNewSolution(StrutSolution oldSolution)
        {
            //for now all we are going to do is take the last failing strut and 
            //and increment its material
            var strutToModify = oldSolution.Failing.Last();
            //increment the material
            var newMat = (Material)((int)strutToModify.Material + 1);

            var materials = oldSolution.Struts.Select(x =>
            {
                if (x.ID == strutToModify.ID)
                {
                    return newMat;
                }
                return x.Material;

            }).ToList();

            var clonedStruts = Strut.ByStrutsAndMaterials(oldSolution.Struts, materials);

            var resultDict = EvaluateStruts(clonedStruts, EvaluatonDelegates);
            var passing = resultDict["passed"] as List<Strut>;
            var failing = resultDict["failed"] as List<Strut>;

            var newSolution = new StrutSolution(clonedStruts, passing, failing, passing.Count() / clonedStruts.Count);
            return newSolution;
        }

    }

    /// <summary>
    /// this class represents a specific configuration of struts
    /// </summary>
    public class StrutSolution
    {
        public List<Strut> Struts { get; private set; }
        public List<Strut> Passing { get; private set; }
        public List<Strut> Failing { get; private set; }
        public double Fitness { get; private set; }

        internal StrutSolution(List<Strut> allStruts, List<Strut> passingStruts, List<Strut> failingStruts, double fitness)
        {
            Struts = allStruts;
            Passing = passingStruts;
            Failing = failingStruts;
            Fitness = fitness;
        }


    }

    /// <summary>
    /// this class can be used to reoresent the result of a test on a strut, 
    /// </summary>
    public class StrutTestResult
    {
        public Strut Strut { get; private set; }
        public bool Passing { get; private set; }
        public double value { get; private set; }
        public Delegate Function { get; private set; }

    }
}
