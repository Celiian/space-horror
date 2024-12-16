using UnityEngine;
public class EnemyAnimator : MonoBehaviour
{
	[SerializeField] private AnimationData animationData;
    [SerializeField] private Zombie zombie;

    private Rigidbody2D _rb;
	private Animator _animator;
	private SpriteRenderer _renderer;
	private Vector2 _previousMovementDirection;
	private Direction _previousDirection = Direction.Down;
	public Direction currentDirection = Direction.Down;

	public bool isLateral => Mathf.Abs(_previousMovementDirection.normalized.x) > 0.75f;
	public bool isUpward => _previousMovementDirection.normalized.y > 0.1f;

	public string currentAnimation = "Idle";

	public enum Direction
	{
		Up,
		Down,
		Left,
		Right
	}

	private void Awake()
	{
		_animator = GetComponent<Animator>();
		_renderer = GetComponent<SpriteRenderer>();
		_rb = GetComponent<Rigidbody2D>();
	}

	private void Update()
	{	
        var movementDirection = (Vector3?)zombie.movementDirection;
		if (movementDirection != null && (Vector2)movementDirection.Value != _previousMovementDirection)
		{
			CacheMovementDirection();
		}
	}

	private void LateUpdate() {
		if (_previousMovementDirection.x != 0) {
			if (currentDirection != _previousDirection && isLateral)_renderer.flipX = false;
			else _renderer.flipX = _previousMovementDirection.x < 0;
		}
	}

	private void CacheMovementDirection()
	{
        var movementDirection = (Vector3?)zombie.movementDirection;
		if (movementDirection == null) return;

		_previousMovementDirection = (Vector2)movementDirection.Value;

		if (_rb.velocity.magnitude > 0.1f)
		{
			if (isLateral)
				currentDirection = _previousMovementDirection.x > 0 ? Direction.Right : Direction.Left;
			else if (isUpward)
				currentDirection = Direction.Up;
			else
				currentDirection = Direction.Down;
		}
	}

	public bool ShouldAnimateAgain()
	{
		bool didChange = currentDirection != _previousDirection;
		if (didChange) _previousDirection = currentDirection;
		return didChange;
	}

	public void PlayAnimation(string animationName, float speedMultiplier = 1f)
	{
		int hash = animationData.GetHash(animationName);
		if (hash != 0)
		{
			_animator.Play(animationName, -1, 0f);
			_animator.speed = 1 * speedMultiplier;
			currentAnimation = animationName;
		}
	}

	public string GetAnimationName(string baseName, string upwardName, string downwardName)
	{
		return currentDirection switch
		{
			Direction.Up => upwardName,
			Direction.Down => downwardName,
			_ => baseName,
		};
	}
}