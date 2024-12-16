using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
	[SerializeField] private AnimationData animationData;
	
	private PlayerMovement _player;
	private Animator _animator;
	private SpriteRenderer _renderer;
	private Vector2 _previousMovementDirection;
	private Direction _previousDirection = Direction.Down;
	private Direction _direction = Direction.Down;

	public bool isLateral => Mathf.Abs(_previousMovementDirection.normalized.x) > 0.75f;
	public bool isUpward => _previousMovementDirection.normalized.y > 0.1f;

	public string currentAnimation = "Idle";

	private enum Direction
	{
		Up,
		Down,
		Lateral
	}

	private void Awake()
	{
		_player = GetComponent<PlayerMovement>();
		_animator = GetComponent<Animator>();
		_renderer = GetComponent<SpriteRenderer>();
	}

	private void Update()
	{	
		if (_player.movementDirection != _previousMovementDirection)
		{
			CacheMovementDirection();
		}
	}

	private void LateUpdate() {
		if (_previousMovementDirection.x != 0) {
			if (_direction != Direction.Lateral)_renderer.flipX = false;
			else _renderer.flipX = _previousMovementDirection.x < 0;
		}
	}

	private void CacheMovementDirection()
	{
		_previousMovementDirection = _player.movementDirection;

		if (_player.isMoving)
		{
			if (isLateral)
				_direction = Direction.Lateral;
			else if (isUpward)
				_direction = Direction.Up;
			else
				_direction = Direction.Down;
		}
	}

	public bool ShouldAnimateAgain()
	{
		bool didChange = _direction != _previousDirection;
		if (didChange) _previousDirection = _direction;
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
		return _direction switch
		{
			Direction.Up => upwardName,
			Direction.Down => downwardName,
			_ => baseName,
		};
	}
}