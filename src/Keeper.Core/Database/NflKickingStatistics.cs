namespace Keeper.Core.Database
{
    public class NflKickingStatistics
    {
        public int PlayerId { get; set; }
        
        public int Season { get; set; }
        
        public int Week { get; set; }
        
        public int? PatMade { get; set; }
        
        public int? FieldGoal0To19Yards { get; set; }
        
        public int? FieldGoal20To29Yards { get; set; }
        
        public int? FieldGoal30To39Yards { get; set; }
        
        public int? FieldGoal40To49Yards { get; set; }
        
        public int? FieldGoal50PlusYards { get; set; }
        
        #region Navigation Properties

        public NflPlayer Player { get; set; }
        
        #endregion
    }
}
