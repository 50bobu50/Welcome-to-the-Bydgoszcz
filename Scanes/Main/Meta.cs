using Godot;
using System;

public class Meta : Sprite3D
{
	Transform RestPose;
	public override void _Ready()
	{
		RestPose = Transform;
	}

	public override void _PhysicsProcess(float delta)
	{
		Transform.Orthonormalized();
		Transform Pozycja = Transform;
		Pozycja.origin = RestPose.origin + new Vector3(0,(Mathf.Cos(Time.GetTicksMsec() * delta * .5f)) * 0.5f,0);
		GD.Print(Pozycja.origin);
		Transform = Pozycja;
	}
}
