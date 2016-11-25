using Microsoft.Xna.Framework;


namespace AndroidWindows
{
    abstract class GameObject : IGameObject
    {
        public bool IsDead { get; set; }
        public Vector2 Position { get; set; }
        public float Radius { get; set; }
        public Vector2 Speed { get; set; }
        public float Rotation { get; set; }

        public bool CollidesWith(IGameObject other)
        {
            return (this.Position - other.Position).LengthSquared() < (Radius + other.Radius) * (Radius + other.Radius);
        }

    }
}
