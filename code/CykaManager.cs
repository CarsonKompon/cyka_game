using System.Collections.Generic;
using Sandbox;

public sealed class CykaManager : Component
{
	[Property] public string LeaderboardName { get; set; } = "highscores";
	public bool Playing { get; private set; } = false;
	public long Score { get; private set; } = 0;
	public long HighScore { get; private set; } = 0;
	public int Evolution { get; private set; } = 1;
	public List<GameObject> Balls { get; private set; } = new();
	public Sandbox.Services.Leaderboards.Board Leaderboard;

	protected override void OnStart()
	{
		StartGame();
	}

	protected override void OnUpdate()
	{
		if ( !Playing && Input.Pressed( "Jump" ) )
		{
			StartGame();
		}
	}

	public void StartGame()
	{
		if ( Playing ) return;

		for ( int i = 0; i < Balls.Count; i++ )
		{
			Balls[i].Destroy();
		}
		Balls.Clear();

		Playing = true;
		Score = 0;
		Evolution = 1;
		FetchLeaderboardInfo();
	}

	public void EndGame()
	{
		if ( !Playing ) return;

		Playing = false;
		Sandbox.Services.Stats.SetValue( "highscore", Score );
		Sandbox.Services.Stats.Increment( "games_played", 1 );
	}

	public void AddBall( GameObject ball )
	{
		Balls.Add( ball );
	}

	public void RemoveBall( GameObject ball )
	{
		Balls.Remove( ball );
	}

	public void AddScore( int size )
	{
		if ( size > Evolution ) Evolution = size;
		var score = 0;
		for ( int i = 0; i < size; i++ )
		{
			score += i + 1;
		}
		Score += score;
		if ( Score > HighScore ) HighScore = Score;
	}

	async void FetchLeaderboardInfo()
	{
		Leaderboard = Sandbox.Services.Leaderboards.Get( LeaderboardName );
		Leaderboard.MaxEntries = 10;
		await Leaderboard.Refresh();
		foreach ( var entry in Leaderboard.Entries )
		{
			if ( entry.SteamId == Game.SteamId )
			{
				HighScore = (long)entry.Value;
			}
		}
	}

}