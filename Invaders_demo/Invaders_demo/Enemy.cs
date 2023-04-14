using Raylib_CsLo;
using System.ComponentModel;
using System.Numerics;

namespace Invaders_demo
{
	internal class Enemy
	{
		public TransformComponent transform;
		public CollisionComponent collision;
		public bool active;

		public Enemy(Vector2 startPosition, Vector2 direction, float speed, int size)
		{
			transform = new TransformComponent(startPosition, direction, speed);
			collision = new CollisionComponent(new Vector2(size, size));
			active = true;
		}
		internal void Update()
		{
			// TODO
			// every now and then shoot at player
			if (active)
			{
				float deltaTime = Raylib.GetFrameTime();
				transform.position += transform.direction * transform.speed * deltaTime;
			}
		}
		internal void Draw()
		{
			if (active)
			{
				Raylib.DrawRectangleV(transform.position, collision.size, Raylib.DARKBROWN);
			}
		}

	}
}
