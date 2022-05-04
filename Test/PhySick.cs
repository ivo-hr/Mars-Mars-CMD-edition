//ENRIQUE JUAN GAMBOA
//Proyecto final FPII


//Physics motor for "realistic simulation" of the astronaut
//This method will be explained in English to ease explanations
namespace PhySick_engine
{
    public class PhySick 
    {
        //Planet specs: gravity and resistance
        private float grav = 10, res = 1, 
        //Time units in planet, time since takeoff and speed limits
            tick = 0, timSinceJump = 0, dangerDropSp = 10, maxf = 13;



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

        //Planet and object constructor
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

        //Object's  current forces
        public float GetForce(int lat0lon1)
        {
            if (lat0lon1 > 0)
            {
                return obj.lonF;
            }
            else return obj.latF;
        }

        //World physics application
        public void MainPhysics()
        {
            //Gravity
            if (timSinceJump > 0)  obj.latF += grav * timSinceJump;

            //Air resistance
            if (obj.lonF < 0)
                obj.lonF += res * timSinceJump ;
            else if (obj.lonF > 0)
                obj.lonF -= res * timSinceJump ;

            //Object movement
            if (timSinceJump > 0)
            {
                obj.lat += obj.latF * timSinceJump;
                obj.lon += obj.lonF * timSinceJump;
            }

            

           
        }

        //Force application upon the object: x = horizontal force applied, y = vertical force applied
        public void ApplyForce(float x, float y)
        {
            obj.latF += y * timSinceJump;
            
            obj.lonF += x * timSinceJump;


            if (obj.latF < -maxf) obj.latF = -maxf;
        }

        //Force cancelation upon the object
        public void CancelForces()
        {
            obj.latF = 0;
            obj.lonF = 0;
        }
        
        //Time elapsed since last jump. Used to increase/decrease forces
        public void TimeSinceFloor(bool count)
        {
            if (count) timSinceJump = (timSinceJump + tick) / 1000;
            else
            {
                timSinceJump = 0;
            }
        }

        //Dangerous entry detection
        public bool FastEntry()
        {
            return obj.latF > dangerDropSp;
        }
    }
}
