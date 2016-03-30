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
            var pass = surf.Area / (Math.Sqrt(strut.LineRepresentation.Length)) < strut.Material.ModulusElasticity;
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
        public static StrutSolution GenerateInitialSolutionByStrutsMaterial(List<Strut> struts, Material initialMaterial, List<Material> possibleMaterials)
        {//TODO we'll probably add an input here for some initial forces for each strut, OR the struts will just carry this info
            //around and so properties will be added, probably a dictionary of forces- or both, but letting us set them
            //explictly here may be good, or they can be set on the initial struts using some update method node...


            //set initial materials
            var newStruts = Strut.ByStrutsAndMaterials(struts, Enumerable.Repeat(initialMaterial, struts.Count).ToList());

            //evaluate this solution
            var resultDict = EvaluateStruts(newStruts, EvaluatonDelegates);
            var passing = resultDict["passed"] as List<Strut>;
            var failing = resultDict["failed"] as List<Strut>;

            var initialSolution = new StrutSolution(newStruts,passing,failing,passing.Count/newStruts.Count,possibleMaterials);
            return initialSolution;
        }

        /// <summary>
        /// this function iterates on a previous solution to produce a new solution
        /// </summary>
        /// <param name="oldSolution"></param>
        /// <returns></returns>
        public static StrutSolution GenerateNewSolution(StrutSolution oldSolution)
        {//TODO IMPLEMENT THIS AS A DESIGN SCRIPT CODEBLOCK WITH SIGNATURE
         //(strutSolution solution,
         //Func<strut,StrutSolution> newSolutionStrategy, 
            // List<Func<strut,bool>> evaluatorFunctions)

            //for now all we are going to do is take the last failing strut and 
            //and increment its material
            var strutToModify = oldSolution.Failing.Last();
            //increment the material by finding the material with great E than the current one
            var newMat = oldSolution.PossibleMaterials.OrderBy(x => x.ModulusElasticity).
                Where(mat => mat.ModulusElasticity > strutToModify.Material.ModulusElasticity).First();

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

            var newSolution = new StrutSolution(clonedStruts, passing, failing, passing.Count / clonedStruts.Count, oldSolution.PossibleMaterials);
            return newSolution;
        }

    }

    /// <summary>
    /// this class represents a specific configuration of struts and the evaluation of this configuration
    /// </summary>
    public class StrutSolution
    {
        public List<Strut> Struts { get; private set; }
        public List<Strut> Passing { get; private set; }
        public List<Strut> Failing { get; private set; }
        public double Fitness { get; private set; }
        public List<Material> PossibleMaterials { get; private set; }

        internal StrutSolution(List<Strut> allStruts, List<Strut> passingStruts, List<Strut> failingStruts, double fitness, List<Material> materials)
        {
            Struts = allStruts;
            Passing = passingStruts;
            Failing = failingStruts;
            Fitness = fitness;
            PossibleMaterials = materials;
        }


    }
}
