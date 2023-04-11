using System.Numerics;

namespace Invaders_demo
{
	internal class CollisionComponent
	{
		public Vector2 size;
		public CollisionComponent(Vector2 size)
		{
			this.size = size;
		}
	}
}
