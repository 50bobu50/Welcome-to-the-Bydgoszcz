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
		Pozycja.origin = RestPose.origin + new Vector3(0,(Mathf.Cos(Time.GetTicksMsec() * delta * .25f)) * 0.25f,0);
		Pozycja.basis = Pozycja.basis.Rotated(Vector3.Up,delta);
		Pozycja.origin = RestPose.origin + new Vector3(0,(Mathf.Cos(Time.GetTicksMsec() * delta * .5f)) * 0.5f,0);
		//GD.Print(Pozycja.origin);
		Transform = Pozycja;
	}
}	
