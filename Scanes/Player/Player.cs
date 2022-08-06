using Godot;
using System;

public class Player : KinematicBody
{
	public int Speed = 14;
	public int FallAcceleration = 75;
	public float MouseSensitivity = 0.05f;
	private Vector3 _velocity = Vector3.Zero;


	private Camera _camera;
	private Spatial _character;
	public override void _Ready()
	{
		_character = GetNode<Spatial>("Pivot");
		_camera = GetNode<Camera>("Position3D/Camera");

		Input.MouseMode = Input.MouseModeEnum.Captured;
	}
	public override void _PhysicsProcess(float delta)
	{
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

		if (direction != Vector3.Zero)
		{
			direction = direction.Normalized();
			_character.LookAt(Translation + direction, Vector3.Up);
		}

		//Ground Velcity
		_velocity.x = direction.x * Speed;
		_velocity.z = direction.z * Speed;
		//Vertical velocity
		_velocity.y -= FallAcceleration * delta;
		// Moving the character
		_velocity = MoveAndSlide(_velocity,Vector3.Up);
	}

	public override void _Input(InputEvent @event)
	{
  		// Mouse in viewport coordinates.
		if (@event is InputEventMouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
		{

			InputEventMouseMotion mouseEvent = @event as InputEventMouseMotion;
			
			_character.RotateX(Mathf.Deg2Rad(mouseEvent.Relative.y * MouseSensitivity));
			RotateY(Mathf.Deg2Rad(-mouseEvent.Relative.x * MouseSensitivity));
			Vector3 cameraRot = _camera.RotationDegrees;
			cameraRot.x = Mathf.Clamp(cameraRot.x, -70, 70);
			_character.RotationDegrees = cameraRot;	
		}
	}
}


