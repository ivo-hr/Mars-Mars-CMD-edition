using System;
using System.Threading;
namespace PhySick_engine
{
    public class PhySick 
    {
        //Universal gravitation constant
        private double univGravConst = 6.67 * Math.Pow(10, -11);

        //Planet specs: gravity and resistance
        private float grav = 10, currGrav = 10, res = 1, tick = 0, timSinceJump = 0, dangerDropSp = 10, maxf = 13;



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
        public PhySick(float gravity, float airResist, float mass, float deathLim, float spLim, float refresh)
        {
            grav = gravity;
            res = airResist;
            obj.mass = mass;
            dangerDropSp = deathLim;
            maxf = spLim;
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

        public float GetForce(int lat0lon1)
        {
            if (lat0lon1 > 0)
            {
                return obj.lonF;
            }
            else return obj.latF;
        }

       

        public void DoPhys(bool applyGrav)
        {
            
        }

        public void MainPhysics()
        {
            
            if (timSinceJump > 0)    obj.latF += grav * timSinceJump;

            //if (obj.latF == 0) obj.latF = ForceTranslator(obj.lonF, -grav);
            //else if (-0.5 < obj.latF && obj.latF < 0) obj.latF = 0f;
            //else if  (0.5 > obj.latF && obj.latF > 0) obj.latF = -0f;

            if (obj.lonF < 0)
                obj.lonF += res * timSinceJump ;
            else if (obj.lonF > 0)
                obj.lonF -= res * timSinceJump ;


            if (timSinceJump > 0)
            {
                obj.lat += obj.latF * timSinceJump;
                obj.lon += obj.lonF * timSinceJump;
            }

            

           
        }

        //x = horizontal force applied, y = vertical force applied
        public void ApplyForce(float x, float y)
        {
            obj.latF += y * timSinceJump;
            
            obj.lonF += x * timSinceJump;


            if (obj.latF < -maxf) obj.latF = -maxf;
        }

        public void CancelForces()
        {
            obj.latF = 0;
            obj.lonF = 0;
        }

        private float ForceTranslator(float startingF,  float forceChange)
        {
            float translated = startingF;;
            if (timSinceJump > 0)
            translated = startingF - ((float)forceChange / timSinceJump);

            return translated/10;
        }

        
        public void TimeSinceFloor(bool count)
        {
            if (count) timSinceJump = (timSinceJump + tick) / 1000;
            else
            {
                timSinceJump = 0;
                currGrav = 0;
            }
        }

        public bool FastEntry()
        {
            return obj.latF > dangerDropSp;
        }
    }
}
