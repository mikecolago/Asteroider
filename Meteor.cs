using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidWindows
{
    enum MeteorType
    {
        Big,
        Medium,
        Small
    }

    class Meteor : GameObject
    {
       public MeteorType Type { get; private set; }
        public float ExplosionScale { get; private set; }
        public Meteor(MeteorType type)
        {
            Type = type;

            switch(Type)
            {
                case MeteorType.Big:
                    Radius = 120;
                    ExplosionScale = 1.0f;
                    break;
                case MeteorType.Medium:
                    Radius = 80;
                    ExplosionScale = 0.5f;
                    break;
                    
                case MeteorType.Small:
                    Radius = 40;
                    ExplosionScale = 0.2f;
                    break;
            }
        }

        public void Update(GameTime gameTime)
        {
            Position += Speed;

            if (Position.X < Globals.GameArea.Left)
                Position = new Vector2(Globals.GameArea.Right, Position.Y);
            if (Position.X > Globals.GameArea.Right)
                Position = new Vector2(Globals.GameArea.Left, Position.Y);

            if (Position.Y < Globals.GameArea.Top)
                Position = new Vector2(Position.X, Globals.GameArea.Bottom);
            if (Position.Y > Globals.GameArea.Bottom)
                Position = new Vector2(Position.X, Globals.GameArea.Top);

            Rotation += 0.04f;
            if (Rotation > MathHelper.TwoPi)
                Rotation = 0;
        }

        public static IEnumerable<Meteor> BreakMeteor(Meteor meteor)
        {
            List<Meteor> meteorList = new List<Meteor>();
            if (meteor.Type == MeteorType.Small)
                return meteorList;

            for (int i = 0; i < 3; i++)
            {
                var angle = (float)Math.Atan2(meteor.Speed.Y, meteor.Speed.X) - MathHelper.PiOver4
                    + MathHelper.PiOver4 * i;
                meteorList.Add(new Meteor(meteor.Type + 1)
                {
                    Position = meteor.Position,
                    Rotation = angle,
                    Speed = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * meteor.Speed.Length()
                });
            }

            return meteorList;
        }
    }
}
