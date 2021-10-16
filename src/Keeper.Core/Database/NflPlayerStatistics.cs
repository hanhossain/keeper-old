namespace Keeper.Core.Database
{
    public class NflPlayerStatistics
    {
        public int PlayerId { get; set; }
        
        public int Season { get; set; }
        
        public int Week { get; set; }
        
        public double FantasyPoints { get; set; }
        
        #region Navigation Properties

        public NflPlayer Player { get; set; }
        
        #endregion
    }
}
