using Godot;
using System;

public class Player : KinematicBody
{
	float speed = 10f;
	float sprintspeed = 20f;
	float acc = 10f;
	float gravity = 10f;
	float mouseSensitivity = 0.1f;
	float jump = 3.5f;

	Vector3 direction = Vector3.Zero;
	Vector3 velocity = Vector3.Zero;
	Vector3 gravityforce = Vector3.Zero;
	Camera camera;
	Position3D head;
	Spatial character;
	RayCast raycastView;
	Camera flashlightcam;

	//Funny music things
	AudioStreamPlayer3D muza;
	float muzaplaybackposition = 0;


	Tween movement_tween;
	[Puppet]
	Vector3 puppet_position;
	[Puppet]
	Vector3 puppet_velocity;
	
	public override void _Ready()
	{
		movement_tween = GetNode<Tween>("Tween");
		Input.MouseMode = Input.MouseModeEnum.Captured;
		character = GetNode<Spatial>("Pivot");
		head = GetNode<Position3D>("Position3D");
		camera = GetNode<Camera>("Position3D/Camera");
		raycastView = GetNode<RayCast>("Position3D/Camera/RayCast");
		flashlightcam = GetNode<Camera>("Position3D/Camera/ViewportContainer/Viewport/Flashlight");
		muza = GetNode<AudioStreamPlayer3D>("Position3D/Muza");
	}

	public override void _PhysicsProcess(float delta)
	{
		if (IsNetworkMaster())
		{
			Movement(delta);
			RpcUnreliable("upddate_state", GlobalTransform.origin, velocity);
		}
		else
		{
			var GTransform = GlobalTransform;
			GTransform.origin = puppet_position;
			velocity.x = puppet_velocity.x;
			velocity.z = puppet_velocity.z;
		}
	} 
	[Puppet]
	public void upddate_state(Vector3 p_position, Vector3 p_velocity)
	{
		puppet_velocity = p_velocity;
		puppet_position = p_position;
		movement_tween.InterpolateProperty(this,"global_transform",GlobalTransform, new Transform(GlobalTransform.basis,p_position),0.1f);
		movement_tween.Start();
	}
	public override void _Process(float delta)
	{
		flashlightcam.GlobalTransform = camera.GlobalTransform;
	}
	public void Movement(float delta)
	{
		//Direction
		direction = Vector3.Zero;
		//	KURWAAA
		float hrot = GlobalTransform.basis.GetEuler().y;
		float forwardInput = Input.GetActionStrength("move_back") - Input.GetActionStrength("move_forward");
		float rightInput = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
		direction = new Vector3(rightInput,0,forwardInput).Rotated(Vector3.Up, hrot).Normalized();
		
		//Gravitation / falling
		if (IsOnFloor())
		{
			gravityforce = Vector3.Zero;
		}
		else
		{
			gravityforce += Vector3.Down * gravity * delta;
			direction = direction * 1.5f;
		}

		//Jump 
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			gravityforce = Vector3.Up * jump;
		}

		//Pick up script
		if (Input.IsActionJustPressed("left_click")) 
		{
			Godot.Object result = raycastView.GetCollider();
			
			if (result != null)
			{
				GD.Print(result.GetType());
				if (result is Area area) 
				{
					if(area.GetParent().GetParent().Name=="MetaHolder")
					{
						(area.GetParent() as Meta).PickUp();
					}
				}
			}
		}

		if (Input.IsActionPressed("sprint"))
		{
			velocity = velocity.LinearInterpolate(direction * sprintspeed, acc * delta);
			flashlightcam.Fov = 90; 
			camera.Fov = 90;
		}
		else
		{
			velocity = velocity.LinearInterpolate(direction * speed, acc * delta);
			flashlightcam.Fov = 70; 
			camera.Fov = 70;
		}
		
		Vector3 xzspeed = new Vector3(velocity.x,0,velocity.z);
		if (Mathf.Round(xzspeed.LengthSquared()) > 220)
		{
			if (muza.Playing == false)
			{
				//muza.Seek();
				muza.Play(muzaplaybackposition);
			}
		}
		else
		{
			if (muza.Playing == true)
			{
				muzaplaybackposition = muza.GetPlaybackPosition();
				muza.Stop();
			}

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