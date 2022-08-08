using Godot;
using System;

public class Player : KinematicBody
{
	float speed = 10f;
	float acc = 10f;
	float gravity = 10f;
	float mouseSensitivity = 0.1f;
	float jump = 3f;

	Vector3 direction = Vector3.Zero;
	Vector3 velocity = Vector3.Zero;
	Vector3 gravityforce = Vector3.Zero;
	//
	Camera camera;
	Position3D head;
	Spatial character;
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		character = GetNode<Spatial>("Pivot");
		head = GetNode<Position3D>("Position3D");
		camera = GetNode<Camera>("Position3D/Camera");
	}

	public override void _PhysicsProcess(float delta)
	{
		//Direction
		direction = Vector3.Zero;
		//	KURWAAA
		float hrot = GlobalTransform.basis.GetEuler().y;
		float forwardInput = Input.GetActionStrength("move_back") - Input.GetActionStrength("move_forward");
		float rightInput = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
		direction = new Vector3(rightInput,0,forwardInput).Rotated(Vector3.Up, hrot).Normalized();


		
		if (IsOnFloor())
		{
			gravityforce = Vector3.Zero;
		}
		else
		{
			gravityforce += Vector3.Down * gravity * delta;
		}

		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			gravityforce = Vector3.Up * jump;
		}

		velocity = velocity.LinearInterpolate(direction * speed, acc * delta);
		velocity = velocity + gravityforce;
		MoveAndSlide(velocity,Vector3.Up);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion mouseMotion)
		{

			RotateY(Mathf.Deg2Rad(-mouseMotion.Relative.x * mouseSensitivity));
			head.RotateX(Mathf.Deg2Rad(-mouseMotion.Relative.y * mouseSensitivity));

			//Limit head rotation
			Vector3 rotDeg = head.RotationDegrees;
			rotDeg.x = Mathf.Clamp(rotDeg.x, -80f, 80f);
			head.RotationDegrees = rotDeg;
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


