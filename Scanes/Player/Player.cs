using Godot;
using System;

public class Player : KinematicBody
{
	public float Speed = 1f;
	public int MaxSpeed = 5;
	public float CounterMometum = 0.2f;
	public int FallAcceleration = 75;
	public float MouseSensitivity = 0.0025f;
	private Vector3 _velocity = Vector3.Zero;


	private Camera _camera;
	private float _rotationX = 0f;
	private float _rotationY = 0f;
	private Spatial _character;
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		_character = GetNode<Spatial>("Pivot");
		_camera = GetNode<Camera>("Position3D/Camera");
	}
	public override void _PhysicsProcess(float delta)
	{
		Transform = Transform.Orthonormalized();

		Vector3 direction = Vector3.Zero;

		if(Input.IsActionPressed("move_right"))
		{
			direction.x += 1f;
		}

		if (Input.IsActionPressed("move_left"))
		{
			direction.x -= 1f;
		}

		if (Input.IsActionPressed("move_back"))
		{
			direction.z += 1f;
		}

		if (Input.IsActionPressed("move_forward"))
		{
			direction.z -= 1f;
		}

		//chodzenie względem kamery
		direction = direction.Rotated(Vector3.Up, _camera.GlobalRotation.y).Normalized();
		//Ground Velcity
		_velocity.x += direction.x * Speed;
		_velocity.z += direction.z * Speed;
		
		
		//GD.Print(direction);
		
		_velocity.x = Mathf.Clamp(_velocity.x,-MaxSpeed,MaxSpeed);
		_velocity.z = Mathf.Clamp(_velocity.z,-MaxSpeed,MaxSpeed);

		//Add Counter momentum
		if (direction.x == 0)
		{
			if (_velocity.x > 0)
			{
				_velocity.x -= Mathf.Clamp(direction.x,0,CounterMometum);
			}
			else if (_velocity.x < 0)
			{
				_velocity.x += Mathf.Clamp(-direction.x,0,CounterMometum);
			}
		}

		if (direction.z == 0)
		{
			if (_velocity.z > 0)
			{
				_velocity.z -= Mathf.Clamp(direction.z,0,CounterMometum);
			}
			else if (_velocity.z < 0)
			{
				_velocity.z += Mathf.Clamp(-direction.z,0,CounterMometum);
			}
		}

		//Vertical velocity
		GD.Print("velocity X:"+Mathf.Round(_velocity.x)+" velocity Z:"+Mathf.Round(_velocity.z));
		_velocity.y -= FallAcceleration * delta;
		
		// Moving the characte
		_velocity = MoveAndSlide(_velocity,Vector3.Up);
	}

	public override void _Input(InputEvent @event)
	{	
  		// Mouse in viewport coordinates.
		if (@event is InputEventMouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			InputEventMouseMotion mouseEvent = @event as InputEventMouseMotion;
			
			_rotationX += -mouseEvent.Relative.x * MouseSensitivity;
			_rotationY += -mouseEvent.Relative.y * MouseSensitivity;
			
			//muj konwertor 👍 
			//Ograniczenie osi Y dla kamery (-50,50 stopni)
			float rotationYDeg = Mathf.Rad2Deg(_rotationY);
			_rotationY = Mathf.Deg2Rad(Mathf.Clamp(rotationYDeg,-50,50));

			//tak 👍
			Transform transform = Transform;
			transform.basis = Basis.Identity;
			Transform = transform;
			
			RotateObjectLocal(Vector3.Up,_rotationX);
			RotateObjectLocal(Vector3.Right,_rotationY);
		}
		else if (@event is InputEventKey eventKey) 
		{
			if (eventKey.Pressed && eventKey.Scancode == (int)KeyList.Escape)
			{
				GetTree().Quit();
			}
		}
	}
}


