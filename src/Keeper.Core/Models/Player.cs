using System;
namespace Keeper.Core.Models
{
    public class Player
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Name => $"{FirstName} {LastName}";
    }
}
