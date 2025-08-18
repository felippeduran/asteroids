namespace Gameplay.Simulation.Runtime
{
    public class ExtraLifeSystem
    {
        public void UpdateExtraLives(ref PlayerState playerState, LivesConfig livesConfig)
        {
            if (playerState.Score >= playerState.NextLifeScore)
            {
                playerState.Lives++;
                playerState.NextLifeScore += livesConfig.ScoreForExtraLife;
            }
        }
    }
}