namespace Keeper.ConsoleApp.Database
{
    public class NflDefensiveStatistics
    {
        public int PlayerId { get; set; }

        public int Season { get; set; }

        public int Week { get; set; }

        public int? Sacks { get; set; }

        public int? Interceptions { get; set; }

        public int? FumblesRecovered { get; set; }

        public int? Safeties { get; set; }

        public int? Touchdowns { get; set; }

        public int? Def2PtRet { get; set; }

        public int? RetTouchdowns { get; set; }

        public int? PointsAllowed { get; set; }

        #region Navigation Properties

        public NflPlayer Player { get; set; }

        #endregion
    }
}

