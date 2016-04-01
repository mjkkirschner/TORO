using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = DSCore.Color;

namespace WireFrameToRobot
{
    /// <summary>
    /// this class represents a type of wood that a strut can be made out of
    /// </summary>
    public class Material
    {
        public double ModulusElasticityX { get; private set; }
        public double ModulusElasticityY { get; private set; }
        public double ModulusElasticityZ { get; private set; }
        public string Name { get; private set; }
        public Color Color { get; private set; }
        public double Density { get; private set; }


        private static double ConvertRange(
    double originalStart, double originalEnd, // original range
    double newStart, double newEnd, // desired range
    double value) // value to convert
        {
            double scale = (double)(newEnd - newStart) / (originalEnd - originalStart);
            return (double)(newStart + ((value - originalStart) * scale));
        }

        public static Material ByModuluiNameDensity(string name,double density, double modulusx, double modulusy, double modulusz, Color color)
        {
            var mat = new Material(name,density, modulusx, modulusy, modulusz, color);
            return mat;
        }

        public Material(string name, double density, double modulusx, double modulusy, double modulusz, Color color)
        {
            this.ModulusElasticityX = modulusx;
            this.ModulusElasticityY = modulusy;
            this.ModulusElasticityZ = modulusz;
            this.Name = name;
            this.Color = color;
        }

        public Material(string name, double density, double modulus, Color color)
        {
            throw new NotImplementedException("GF. needs to look at this, go get him :D");

            this.ModulusElasticityX = modulus * (ConvertRange(30, 70, .1, 1.2,density));
            this.ModulusElasticityY = modulus * (ConvertRange(30, 70, .1, 1.2, density));
            this.ModulusElasticityZ = modulus * (ConvertRange(30, 70, .1, 1.2, density));
            this.Name = name;
            this.Color = color;
        }

        public static Material Ash()
        {
            return new Material("ash",43, 1740000, 1740000, 1740000, Color.ByARGB(255, 255, 0, 0));
        }
        public static Material Spruce()
        {
            return new Material("spruce",34, 1400000, 1400000, 1400000, Color.ByARGB(255, 75, 125, 50));
        }
        public static Material Birch()
        {
            return new Material("birch",57, 2010000, 2010000, 2010000, Color.ByARGB(255, 255, 255, 200));
        }
        public static Material Oak()
        {
            return new Material("oak",63, 1762000, 1762000, 1762000, Color.ByARGB(255, 20, 20, 20));
        }
        public static Material Beech()
        {
            return new Material("beech",54, 1720000, 1720000, 1720000, Color.ByARGB(255, 200, 0, 200));
        }

        public static Material Steel()
        {
            return new Material("Steel", 490.059490523, 10000000000, 10000000000, 10000000000, Color.ByARGB(200, 255, 255, 255));
        }
    }



}
