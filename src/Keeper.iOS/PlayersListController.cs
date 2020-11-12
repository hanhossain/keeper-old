using System;
using Keeper.iOS.Extensions;
using Foundation;
using UIKit;
using System.Collections.Generic;
using Keeper.Core.Models;
using Keeper.Core.Services;
using System.Linq;

namespace Keeper.iOS
{
    public class PlayersListController : UITableViewController
    {
        private const string CellId = "playerCell";

        private readonly PlayerService _playerService = new PlayerService();

        private Dictionary<char, List<Player>> _players = new Dictionary<char, List<Player>>();
        private List<char> _sections = new List<char>();

        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();
            Title = "Players";
            TableView.RegisterClassForCellReuse<UITableViewCell>(CellId);

            var players = await _playerService.GetPlayersAsync();
            (_players, _sections) = GetPlayersAndSections(players);
            
            TableView.ReloadData();
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return _sections.Count();
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return _players[_sections[(int)section]].Count;
        }

        public override string TitleForHeader(UITableView tableView, nint section)
        {
            return _sections[(int)section].ToString();
        }

        public override string[] SectionIndexTitles(UITableView tableView)
        {
            return _sections.Select(x => x.ToString()).ToArray();
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(CellId, indexPath);

            var section = _sections[indexPath.Section];
            var player = _players[section][indexPath.Row];

            cell.TextLabel.Text = player.Name;

            return cell;
        }

        private (Dictionary<char, List<Player>>, List<char>) GetPlayersAndSections(List<Player> originalPlayers)
        {
            var players = new Dictionary<char, List<Player>>();
            var sections = new List<char>();

            foreach (var player in originalPlayers)
            {
                var firstLetter = char.ToUpper(player.LastName.First());
                if (firstLetter >= '0'  && firstLetter <= '9')
                {
                    firstLetter = '#';
                }

                if (sections.LastOrDefault() != firstLetter)
                {
                    sections.Add(firstLetter);
                }

                if (!players.TryGetValue(firstLetter, out var playersInSection))
                {
                    playersInSection = new List<Player>();
                    players[firstLetter] = playersInSection;
                }

                playersInSection.Add(player);
            }

            return (players, sections);
        }
    }
}
