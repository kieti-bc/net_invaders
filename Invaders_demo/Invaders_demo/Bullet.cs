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
		public bool active;
		public TransformComponent transform;
		public CollisionComponent collision;
		SpriteRendererComponent spriteRenderer;

		public Bullet(Vector2 startPosition, Vector2 direction, float speed, int size, Texture image, Color color)
		{
			transform = new TransformComponent(startPosition, direction, speed);
			collision = new CollisionComponent(new Vector2(size, size));
			spriteRenderer = new SpriteRendererComponent(image, color, transform, collision);
		}

		public void Reset(Vector2 startPosition, Vector2 direction, float speed, int size)
		{
			transform.position = startPosition;
			transform.direction = direction;
			transform.speed = speed;
			collision.size = new Vector2(size, size);

			active = true;
		}

		public void Update()
		{
			transform.position += transform.direction * transform.speed * Raylib.GetFrameTime();
		}
		public void Draw()
		{
			spriteRenderer.Draw();
		}
	}
}
