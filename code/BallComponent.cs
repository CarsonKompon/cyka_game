using System.Collections.Generic;
using Sandbox;

public sealed class BallComponent : Component, Component.ICollisionListener
{
	static BallComponent instance;

	public CykaManager Manager;
	[Property] public int Size { get; set; } = 1;
	[Property] public List<string> Emojis { get; set; } = new();

	float colliderSize = 10f;
	float fontScale = 0f;
	SphereCollider collider;
	TextRenderer textRenderer;

	protected override void OnAwake()
	{
		instance = this;
	}

	protected override void OnStart()
	{
		collider = Components.Get<SphereCollider>( FindMode.EverythingInSelfAndChildren );
		colliderSize = collider.Radius;

		textRenderer = Components.Get<TextRenderer>( FindMode.EverythingInSelfAndChildren );
		textRenderer.Text = GetEmoji();
		fontScale = textRenderer.Scale;

		var rigidbody = Components.Get<Rigidbody>( FindMode.EverythingInSelfAndChildren );
		rigidbody.CollisionUpdateEventsEnabled = true;
	}

	protected override void OnUpdate()
	{
		colliderSize = MathX.LerpTo( colliderSize, GetSize(), Time.Delta * 10f );
		collider.Radius = colliderSize;

		fontScale = MathX.LerpTo( fontScale, GetFontScale(), Time.Delta * 10f );
		textRenderer.Scale = fontScale;
	}

	protected override void OnDestroy()
	{
		Manager.RemoveBall( GameObject );
	}

	public void OnCollisionUpdate( Collision collision )
	{
		var other = collision.Other.GameObject.Components.Get<BallComponent>();
		if ( other != null && other.Size == Size )
		{
			this?.Grow();
			other.GameObject.DestroyImmediate();
		}
	}

	public void OnCollisionStop( CollisionStop collision )
	{

	}

	public void Grow()
	{
		Size++;
		if ( Size >= Emojis.Count )
		{
			Sandbox.Services.Achievements.Unlock( "largest_merge" );
		}
		Manager.AddScore( Size );
		Sandbox.Services.Stats.Increment( "merge_count", 1 );
		if ( textRenderer != null )
			textRenderer.Text = GetEmoji();
	}

	public string GetEmoji()
	{
		var size = Size - 1;
		if ( size >= Emojis.Count || size < 0 || Emojis.Count == 0 )
			return "❓";

		return Emojis[size];
	}

	public static string GetEmoji( int size )
	{
		if ( instance is null ) return "❓";

		size -= 1;
		if ( size >= instance.Emojis.Count || size < 0 || instance.Emojis.Count == 0 )
			return "❓";

		return instance.Emojis[size];
	}

	public float GetFontScale()
	{
		return (GetSize()) / 64f;
	}

	public float GetSize()
	{
		return 24f * (Size);
	}


}