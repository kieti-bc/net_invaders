using System.Numerics;

namespace Invaders_demo
{
	internal class TransformComponent
	{
		public Vector2 position;
		public Vector2 direction;
		public float speed;

		public TransformComponent(Vector2 position, Vector2 direction, float speed)
		{
			this.position = position;
			this.direction = direction;
			this.speed = speed;
		}
	}
}
