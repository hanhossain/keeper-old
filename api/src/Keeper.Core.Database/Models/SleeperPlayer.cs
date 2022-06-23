using System.ComponentModel.DataAnnotations.Schema;

namespace Keeper.Core.Database.Models
{
    public class SleeperPlayer
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get; set; }

        public string Position { get; set; }

        public bool Active { get; set; }

        public string Team { get; set; }

        public string Status { get; set; }

        [ForeignKey(nameof(NflPlayer))]
        public int? NflId { get; set; }

        #region Navigation Properties

        public NflPlayer NflPlayer { get; set; }

        #endregion
    }
}

