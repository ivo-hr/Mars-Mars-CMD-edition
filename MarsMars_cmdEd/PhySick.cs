using System;

namespace PhySick_engine
{
    public class PhySick 
    {
        //Universal gravitation constant
        private double univGravConst = 6.67 * Math.Pow(10, -11);

        //Planet specs: gravity and resistance
        private float grav  = 10;
        private float res = 0;

        //Simulated object: mass, downward  applied  force,  horizontal speed, latitude and longitude
        struct MassObj
        {
            public float objMass,
                        objForce,
                        objSpeed,
                        lat,
                        lon;
        }
        MassObj obj = new MassObj();

        //Constructor
        public PhySick(float gravity, float airResist, float mass)
        {
            grav = gravity;
            res = airResist;
            obj.objMass = mass;
        }

        //Object position (lat and lon) get/set
        public float[] objPos
        {
            get
            {
                return new float[2] { this.obj.lat, this.obj.lon };
            }
            set
            {
                this.obj.lat = value[0];
                this.obj.lon = value[1];
            }
        }

       

        public void DoPhys(bool applyGrav)
        {

        }

        private void MainPhysics()
        {

        }


        public void ApplyForce(float x, float y)
        {
            float newY = y;
            obj.objForce = ForceTranslator(obj.objForce, ref newY);
        }

        private float ForceTranslator(float startingF,  ref float forceChange)
        {
            float translated = 0;

            translated = startingF - (float)Math.Pow(forceChange, 2);

            return translated;
        }

        

    }
}
