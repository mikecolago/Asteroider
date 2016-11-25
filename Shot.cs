using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidWindows
{
    class Shot : GameObject
    {
        public Shot()
        {
            Radius = 16;
        }       
        public void Update(GameTime time)
        {
            Position += Speed;
            Rotation += 0.08f;

            if(Rotation > MathHelper.TwoPi)
            {
                Rotation = 0;
            }
        }
    }
}
