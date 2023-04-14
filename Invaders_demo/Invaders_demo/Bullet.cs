using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Raylib_CsLo;
using System.Threading.Tasks;

namespace Invaders_demo
{
	internal class Bullet
	{
		public bool isActive;
		public TransformComponent transform;
		public CollisionComponent collision;

		public Bullet(Vector2 startPosition, Vector2 direction, float speed, int size)
		{
			Reset(startPosition, direction, speed, size);	 
		}

		public void Reset(Vector2 startPosition, Vector2 direction, float speed, int size)
		{
			isActive = true;

			transform = new TransformComponent(startPosition, direction, speed);
			collision = new CollisionComponent(new Vector2(size, size));
		}

		public void Update()
		{
			transform.position += transform.direction * transform.speed * Raylib.GetFrameTime();
		}
		public void Draw()
		{
			Raylib.DrawRectangleV(transform.position, collision.size, Raylib.RED);
		}
	}
}
