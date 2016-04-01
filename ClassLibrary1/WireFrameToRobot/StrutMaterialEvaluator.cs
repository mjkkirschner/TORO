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
        static bool equation1(StrutLoadPackage strutLoadPackage)

        {
            var surf = Autodesk.DesignScript.Geometry.Surface.ByPatch(strutLoadPackage.Strut.Profile);
            var pass = (Math.Sqrt(strutLoadPackage.Strut.Material.ModulusElasticityX)*surf.Area)/ strutLoadPackage.Strut.LineRepresentation.Length > strutLoadPackage.Axial;
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
        public static List<Func<StrutLoadPackage, bool>> EvaluatonDelegates
        {
            get
            {
                return typeof(EvalautionMethods).GetMethods(BindingFlags.Static|BindingFlags.NonPublic|BindingFlags.FlattenHierarchy).Select
                          (x => Delegate.CreateDelegate(typeof(Func<StrutLoadPackage, bool>), x)).Cast<Func<StrutLoadPackage, bool>>().ToList();
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
        public static Dictionary<string, object> EvaluateStruts(List<StrutLoadPackage> strutSolution, List<Func<StrutLoadPackage, bool>> functionsToTest)
        {

            var outputDict = new Dictionary<string, object>();

            var passed = strutSolution.Where(strutPackge => functionsToTest.All(test => test.Invoke(strutPackge) == true)).Select(x=>x.Strut).ToList();
            var failed = strutSolution.Where(strutPackage => functionsToTest.Any(test => test.Invoke(strutPackage) == false)).Select(x => x.Strut).ToList();

            outputDict.Add("passed", passed);
            outputDict.Add("failed", failed);

            return outputDict;
        }


        /// <summary>
        /// create a first solution from a material and struts, all struts are set to this material
        /// and then the solution is evaluated
        /// </summary>
        /// <param name="strutPackages"></param>
        /// <param name="material"></param>
        /// <returns></returns>
        public static StrutSolution GenerateInitialSolutionByStrutsPackagesAndMaterial(List<StrutLoadPackage> strutPackages, Material initialMaterial, List<Material> possibleMaterials)
        {//TODO we'll probably add an input here for some initial forces for each strut, OR the struts will just carry this info
            //around and so properties will be added, probably a dictionary of forces- or both, but letting us set them
            //explictly here may be good, or they can be set on the initial struts using some update method node...

            var oldStruts = strutPackages.Select(x => x.Strut).ToList();
            //set initial materials
            var newStruts = Strut.ByStrutsAndMaterials(oldStruts, Enumerable.Repeat(initialMaterial, strutPackages.Count).ToList());
            //generate new packakges from the new struts and old forces
            var newPackages = strutPackages.Zip(newStruts, (x, y) => x.UpdateStrut(y)).ToList();

            //evaluate this solution
            var resultDict = EvaluateStruts(newPackages, EvaluatonDelegates);
            var passing = resultDict["passed"] as List<Strut>;
            var failing = resultDict["failed"] as List<Strut>;

            var initialSolution = new StrutSolution(newPackages,passing,failing,passing.Count/newStruts.Count,possibleMaterials);
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
            var newMat = oldSolution.PossibleMaterials.OrderBy(x => x.ModulusElasticityX).
                Where(mat => mat.ModulusElasticityX > strutToModify.Material.ModulusElasticityX).FirstOrDefault();

            if (newMat == null)
            {
                //we couldnt find a material that would make this strut pass - use the super material
                newMat = Material.Steel();
            }

            //build a new list of materials to set the struts to
            var materials = oldSolution.StrutLoadPackages.Select(x =>
            {
                //only modify one strut
                if (x.Strut.ID == strutToModify.ID)
                {
                    return newMat;
                }
                return x.Strut.Material;

            }).ToList();

            var clonedStruts = Strut.ByStrutsAndMaterials(oldSolution.StrutLoadPackages.Select(x=>x.Strut).ToList() , materials);

            //generate new packakges from the new struts and old forces
            var newPackages = oldSolution.StrutLoadPackages.Zip(clonedStruts, (x, y) => x.UpdateStrut(y)).ToList();

            var resultDict = EvaluateStruts(newPackages, EvaluatonDelegates);
            var passing = resultDict["passed"] as List<Strut>;
            var failing = resultDict["failed"] as List<Strut>;

            var newSolution = new StrutSolution(newPackages, passing, failing, passing.Count / clonedStruts.Count, oldSolution.PossibleMaterials);
            return newSolution;
        }

    }
    /// <summary>
    /// this class represents a strut and a set of forces placed upon it, 
    /// one would use this class to evaluate a specific strut to see if it passes
    /// a load check or not
    /// </summary>
    public class StrutLoadPackage : IDisposable
    {
        public Strut Strut { get; private set; }
        public double Axial { get; private set; }
        public double Moment { get; private set; }
        public double Shear { get; private set; }

        private StrutLoadPackage(Strut strut, double axial, double moment, double shear)
        {
            this.Strut = strut;
            this.Axial = axial;
            this.Moment = moment;
            this.Shear = shear;
        }

        /// <summary>
        /// constructs a package representing a strut with specific load 
        /// </summary>
        /// <param name="strut"></param>
        /// <param name="axial"></param>
        /// <param name="moment"></param>
        /// <param name="shear"></param>
        /// <returns></returns>
        public static StrutLoadPackage ByStrutForces(Strut strut, double axial = 0, double moment = 0, double shear = 0)
        {
            return new StrutLoadPackage(strut.Clone() as Strut, axial, moment, shear);
        }
        public StrutLoadPackage UpdateStrut(Strut strut)
        {
            return new StrutLoadPackage(strut, this.Axial,this.Moment, this.Shear);
        }

        public void Dispose()
        {
            Strut.Dispose();
        }
    }

    /// <summary>
    /// this class represents a specific configuration of struts and the evaluation of this configuration
    /// </summary>
    public class StrutSolution:IDisposable
    {
        public List<StrutLoadPackage> StrutLoadPackages { get; private set; }
        public List<Strut> Passing { get; private set; }
        public List<Strut> Failing { get; private set; }
        public double Fitness { get; private set; }
        public List<Material> PossibleMaterials { get; private set; }

        internal StrutSolution(List<StrutLoadPackage> allPackages, List<Strut> passingStruts, List<Strut> failingStruts, double fitness, List<Material> materials)
        {
            StrutLoadPackages = allPackages;
            Passing = passingStruts;
            Failing = failingStruts;
            Fitness = fitness;
            PossibleMaterials = materials;
        }

        public void Dispose()
        {
            StrutLoadPackages.ForEach(x => x.Dispose());
        }
    }
}
