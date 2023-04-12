using Raylib_CsLo;
using System.ComponentModel;
using System.Numerics;

namespace Invaders_demo
{
	internal class Enemy
	{
		public TransformComponent transform;
		public CollisionComponent collision;

		public Enemy(Vector2 startPosition, Vector2 direction, float speed, int size)
		{
			transform = new TransformComponent(startPosition, direction, speed);
			collision = new CollisionComponent(new Vector2(size, size));
		}
		internal void Draw()
		{
			Raylib.DrawRectangleV(transform.position, collision.size, Raylib.DARKBROWN);
		}

		internal void Update()
		{
			// TODO
			// Move from side to side
			// every now and then shoot at player???
			float deltaTime = Raylib.GetFrameTime();
			transform.position += transform.direction * transform.speed * deltaTime;
		}
	}
}
