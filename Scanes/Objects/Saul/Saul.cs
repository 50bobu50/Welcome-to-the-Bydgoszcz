using Godot;
using System;

public class Saul : KinematicBody
{
	float speed = 10f;
	float acc = 10f;
	float gravity = 10f;

	Vector3 direction = Vector3.Zero;
	Vector3 velocity = Vector3.Zero;
	Vector3 gravityforce = Vector3.Zero;

	NavigationAgent navAgent;
	KinematicBody target = null;
	Vector3 targetPos;
	Godot.Collections.Array targets;
	Godot.Collections.Array Empty;
	
	public override void _Ready()
	{
		navAgent = GetNode<NavigationAgent>("NavigationAgent");
		navAgent.SetNavigation(GetNode<Navigation>("/root/Main/Navigation"));
	}

	public override void _PhysicsProcess(float delta)
	{
		if(target == null)
		{
			return;
		}
		while(IsInstanceValid(target as Node))
		{
			targetPos = target.GlobalTransform.origin;
			velocity = Vector3.Zero;
			if (targetPos != Vector3.Zero)
			{
				navAgent.SetTargetLocation(targetPos);
			}
			else
			{
				navAgent.SetTargetLocation(GlobalTransform.origin);
			}
			Vector3 nextPoint = navAgent.GetNextLocation();
			direction = (nextPoint - GlobalTransform.origin).Normalized();
			//GRAVITY
			if (IsOnFloor())
			{
				gravityforce = Vector3.Zero;
			}
			else
			{
				gravityforce += Vector3.Down * gravity * delta;
			}
			//Velocity things
			velocity = velocity.LinearInterpolate(direction * speed, acc * delta);
			velocity = velocity + gravityforce;
			//LOOK AT FUNCTION
			Vector3 lookDirection = GlobalTransform.origin - direction;
			lookDirection.y = GlobalTransform.origin.y;
			if (lookDirection != GlobalTransform.origin)
			{
				LookAt(lookDirection,Vector3.Up);
			}
			MoveAndSlide(velocity, Vector3.Up);
			break;
		}
	}
	public void _on_Timer_timeout()
	{
		if(GetTree().IsNetworkServer() || GetNodeOrNull("../Players/1")==null)
		{
			float targetDistance = 0f;
			targets = GetNode("../Players").GetChildren();
			//Target location
			if(targets==Empty)
			{
				return;
			}
			foreach (KinematicBody player in targets)
			{
				if(player.GlobalTransform.origin.DistanceTo(GlobalTransform.origin)<targetDistance || target is null)
				{
					targetDistance=player.GlobalTransform.origin.DistanceTo(GlobalTransform.origin);
					target = player;
				}
			}
			Rpc("setTarget",target);
		}
	}
	[Sync]
	public void setTarget(KinematicBody targetM)
	{
		GD.Print("aaa");
		target = targetM;
	}
	public void _on_Area_body_entered(object body)
	{
		Node bodyNode = body as Node;
		if(bodyNode.GetParent().Name=="Players")
		{
			KillPlayer();
		}
	}
	void KillPlayer()
	{
		Rpc("PlayerDied");
		(GetNode("../UI") as CanvasItem).Visible = false;
		Input.MouseMode = Input.MouseModeEnum.Visible;
		GetTree().ChangeScene("res://Scanes/Lobby/LOBBY.tscn");
	}
	[Sync]
	public void PlayerDied()
	{
		int id = GetTree().GetRpcSenderId();
		GetTree().CallGroup("Lobby","player_died",id);
		string path = "../Players/"+id;
		GetNode(path).QueueFree();
	}
}
