using Godot;
using System;

public class Saul : KinematicBody
{
	float speed = 15f;
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

	//Aanimacja FSSAFNOSNFOSABFNO
	AnimationTree blendtree;
	AnimationNodeStateMachinePlayback blendmode;
	

	public override void _Ready()
	{
		navAgent = GetNode<NavigationAgent>("NavigationAgent");
		navAgent.SetNavigation(GetNode<Navigation>("/root/Main/Navigation"));
				//Animacja
		blendtree = GetNode<AnimationTree>("AnimationTree");
		blendmode = (AnimationNodeStateMachinePlayback)blendtree.Get("parameters/playback");
	}

	public override void _PhysicsProcess(float delta)
	{
		if(target == null)
		{
			return;
		}
		if(IsInstanceValid(target as Node))
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

			//Velocity check
			Vector3 xzspeed = new Vector3(velocity.x,0,velocity.z);
			float playerspeed = xzspeed.LengthSquared();

			String currentNodeName = blendmode.GetCurrentNode();
			Boolean isPlaying = blendmode.IsPlaying();


			GD.Print(playerspeed);

			if (playerspeed > 50f)
			{
				if (currentNodeName != "Run")
				{
					blendmode.Travel("Run");
				}
			}
			else if (playerspeed > 1f)
			{
				if (currentNodeName != "Walk")
				{
					blendmode.Travel("Walk");
				}
			}
			else if (playerspeed < 0.5f)
			{
				if (currentNodeName != "Default")
				{
					blendmode.Travel("Default");
				}
			}



			//LOOK AT FUNCTION
			Vector3 lookDirection = GlobalTransform.origin - direction;
			lookDirection.y = GlobalTransform.origin.y;
			if (lookDirection != GlobalTransform.origin)
			{
				LookAt(lookDirection,Vector3.Up);
			}
			MoveAndSlide(velocity, Vector3.Up);
		}
	}
	public void _on_Timer_timeout()
	{
		if(GetTree().IsNetworkServer() || GetNodeOrNull("../Players/1")==null)
		{
			float targetDistance = 0f;
			targets = GetNode("../Players").GetChildren();
			target = null;
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
			if(GetTree().IsNetworkServer())
			{
				if (target != null)
				{
					Rpc("setTarget",target.GetPath());
				}
			}
		}
	}
	[Sync]
	public void setTarget(Godot.NodePath targetM)
	{
		target = GetNode(targetM) as KinematicBody;
	}
	public void _on_Area_body_entered(object body)
	{
		Node bodyNode = body as Node;
		if(bodyNode.GetParent().Name=="Players")
		{
			blendmode.Travel("Attack");
			KillPlayer();
		}
	}
	void KillPlayer()
	{
		Rpc("PlayerDied");
		Input.MouseMode = Input.MouseModeEnum.Visible;
		foreach(Node child in GetTree().Root.GetChildren())
		{
			GD.Print(child.Name);
			if(child.Name!="Network")
			{
				child.QueueFree();
			}
		}
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
