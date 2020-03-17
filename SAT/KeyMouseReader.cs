using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAT
{
    static class KeyMouseReader
    {
        public static MouseState mouseState, oldMouseState = Mouse.GetState();
        public static Point mousePos, oldMousePos;


        public static bool LMB_Hold()
        {
            return mouseState.LeftButton == ButtonState.Pressed;
        }
        public static bool LMB_Click()
        {
            return LMB_Hold() && oldMouseState.LeftButton == ButtonState.Released;
        }
            
        public static void Update()
        {
            oldMouseState = mouseState;
            mouseState = Mouse.GetState();

            oldMousePos = oldMouseState.Position;
            mousePos = mouseState.Position;
        }
    }
}
