using Godot;
using System;

public class Meta : Sprite3D
{
	private Transform RestPose;
	private Area ClickDetection;
	public override void _Ready()
	{
		RestPose = Transform;
		ClickDetection = GetNode<Area>("Area");
	}

	public override void _PhysicsProcess(float delta)
	{
		Transform Pozycja = Transform;
		Pozycja.origin = RestPose.origin + new Vector3(0,(Mathf.Cos(Time.GetTicksMsec() * delta * .5f)) * 0.5f,0);
		Transform = Pozycja;
	}
}	
