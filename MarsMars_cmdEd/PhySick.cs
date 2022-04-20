using System;

namespace PhySick_engine
{
    public class PhySick 
    {
        //Universal gravitation constant
        private double univGravConst = 6.67 * Math.Pow(10, -11);

        //Planet specs: gravity and resistance
        private float grav = 10, res = 1, tick = 0;


        //Simulated object: mass, downward  applied  force,  horizontal speed, latitude and longitude
        struct MassObj
        {
            public float mass,
                        latF,
                        lonF,
                        lat,
                        lon;

        }
        MassObj obj = new MassObj();

        //Constructor
        public PhySick(float gravity, float airResist, float mass, float refresh)
        {
            grav = gravity;
            res = airResist;
            obj.mass = mass;
            tick = refresh;

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
            float sSq = (float)Math.Pow(tick / 1000, 2);

            obj.latF = ForceTranslator(obj.latF, grav);

            if (obj.lonF < 0)
                obj.lonF += res / sSq;
            else if (obj.lonF < 0)
                obj.lonF -= res / sSq;


            obj.lat = obj.latF * (tick / 1000);
            obj.lon = obj.lonF * (tick / 1000);
        }


        public void ApplyForce(float x, float y, bool applyGrav)
        {
            float newY = y, newX = x - grav;
            obj.latF = ForceTranslator(obj.latF, newY);
            obj.lonF = ForceTranslator(obj.lonF, newX);
        }

        private float ForceTranslator(float startingF,  float forceChange)
        {
            float translated = 0;

            translated = startingF - ((float)Math.Pow(forceChange, 2) / (float)Math.Pow(tick / 1000, 2));

            return translated;
        }

        

    }
}
