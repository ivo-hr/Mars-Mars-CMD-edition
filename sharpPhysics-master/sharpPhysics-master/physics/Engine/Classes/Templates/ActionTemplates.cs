using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using physics.Engine.Structs;

namespace physics.Engine.Classes.ObjectTemplates
{
    public static class ActionTemplates
    {
        public static void launch(PhysicsSystem physSystem, PhysicsObject physObj, PointF StartPointF, PointF EndPointF)
        {
            physSystem.ActivateAtPoint(StartPointF);
            Vec2 delta = (new Vec2 { X = EndPointF.X, Y = EndPointF.Y } -
                          new Vec2 { X = StartPointF.X, Y = StartPointF.Y });
            physSystem.AddVelocityToActive(-delta);
        }

        public static void changeShader(PhysicsSystem physSystem, aShader shader)
        {
            foreach(PhysicsObject obj in physSystem.GetMoveableObjects())
            {
                obj.Shader = shader;
            }
        }

        public static void PopAndMultiply(PhysicsSystem physSystem)
        {
            foreach(PhysicsObject obj in physSystem.GetMoveableObjects())
            {
                physSystem.ActivateAtPoint(new PointF(obj.Center.X, obj.Center.Y));
                var velocity = obj.Velocity;
                var origin = obj.Center;
                physSystem.RemoveActiveObject();
                physSystem.SetVelocity(ObjectTemplates.CreateSmallBall(origin.X, origin.Y), velocity);
                physSystem.SetVelocity(ObjectTemplates.CreateSmallBall(origin.X, origin.Y), velocity);
            }
        }
    }
}
