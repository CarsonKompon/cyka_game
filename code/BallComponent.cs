using System.Collections.Generic;
using Sandbox;

public sealed class BallComponent : Component, Component.ICollisionListener
{
	static BallComponent instance;

	public CykaManager Manager;
	[Property] public int Size { get; set; } = 1;
	[Property] public List<string> Emojis { get; set; } = new();

	SphereCollider collider;
	TextRenderer textRenderer;

	public TimeSince TimeSinceCreated = 0f;

	protected override void OnAwake()
	{
		instance = this;

		WorldScale = 0;
	}

	protected override void OnStart()
	{
		collider = Components.Get<SphereCollider>( FindMode.EverythingInSelfAndChildren );
		textRenderer = Components.Get<TextRenderer>( FindMode.EverythingInSelfAndChildren );

		textRenderer.Text = GetEmoji();
		collider.Radius = GetSize();
		textRenderer.Scale = GetFontScale();
	}

	protected override void OnUpdate()
	{
		WorldScale = WorldScale.LerpTo( 0.5f, 10 * Time.Delta );
	}

	protected override void OnDestroy()
	{
		Manager.RemoveBall( this );
	}

	public void OnCollisionStart( Collision collision )
	{
		var other = collision.Other.GameObject.Components.Get<BallComponent>();
		if ( other != null && other.Size == Size )
		{
			this?.Grow();
			other.GameObject.DestroyImmediate();
		}
	}

	public void OnCollisionUpdate( Collision collision )
	{

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