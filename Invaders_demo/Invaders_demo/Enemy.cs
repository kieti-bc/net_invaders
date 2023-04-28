using Raylib_CsLo;
using System.Numerics;

namespace Invaders_demo
{
	internal class Enemy
	{
		public TransformComponent transform;
		public CollisionComponent collision;
		SpriteRendererComponent spriteRenderer;

		public bool active;
		public int scoreValue;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="startPosition"></param>
		/// <param name="direction"></param>
		/// <param name="speed"></param>
		/// <param name="size"></param>
		/// <param name="score">How many points for destroying this enemy</param>
		public Enemy(Vector2 startPosition, Vector2 direction, float speed, int size, Texture image, int score)
		{
			transform = new TransformComponent(startPosition, direction, speed);
			collision = new CollisionComponent(new Vector2(size, size));
			spriteRenderer = new SpriteRendererComponent(image, Raylib.RED, transform, collision);
			active = true;
			scoreValue = score;
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
				spriteRenderer.Draw();
			}
		}

	}
}
