using System.Linq;
using Sandbox;

public sealed class DeathZone : Component
{
	[Property] CykaManager Manager { get; set; }
	[Property] public float DeathTime { get; set; } = 5f;
	public bool BallAboveLine => Manager.Balls.Any( ball => ball.TimeSinceCreated > 1f && ball.WorldPosition.z > WorldPosition.z );

	public float TimeInZone = 0f;
	float textScale = 0f;

	TextRenderer textRenderer;

	protected override void OnStart()
	{
		textRenderer = Components.Get<TextRenderer>();
		textRenderer.Scale = 0;
	}


	protected override void OnUpdate()
	{
		if ( !Manager.Playing ) return;

		if ( BallAboveLine )
		{
			TimeInZone += Time.Delta;

			if ( TimeInZone >= DeathTime )
			{
				Manager?.EndGame();
			}

			if ( TimeInZone >= 1f )
			{
				textScale = textScale.LerpTo( 1f, Time.Delta * 10f );
			}
			else
			{
				textScale = textScale.LerpTo( 0f, Time.Delta * 10f );
			}
		}
		else
		{
			TimeInZone = 0f;
			textScale = textScale.LerpTo( 0f, Time.Delta * 10f );
		}


		textRenderer.Scale = textScale;
	}
}