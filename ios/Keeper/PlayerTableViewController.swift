//
//  PlayerTableViewController.swift
//  Keeper
//
//  Created by Han Hossain on 6/23/22.
//

import UIKit

class PlayerTableViewController: UITableViewController, UISearchResultsUpdating {
    private var sectionHeaders = [Character]()
    private var players = [Character: [Player]]()
    private var filteredPlayers = [Character: [Player]]()
    private var seasonStatistics = [String: SeasonStatistics]()
    
    private let cellId = "playerCell"
    private let sleeperClient = SleeperClient()

    override func viewDidLoad() {
        super.viewDidLoad()
        title = "Players"
        view.backgroundColor = .systemBackground
        tableView.register(SubtitleRightDetailTableViewCell.self, forCellReuseIdentifier: cellId)
        
        let searchController = UISearchController()
        searchController.searchResultsUpdater = self
        navigationItem.searchController = searchController
        
        Task {
            async let players: () = loadPlayers()
            async let seasonStatistics: () = getSeasonStatistics()
            
            await players
            await seasonStatistics
            
            tableView.reloadData()
        }
    }
    
    private func getSeasonStatistics() async {
        do {
            let seasonStatistics = try await sleeperClient.getSeasonStatistics()
            
            var statistics = [String: SeasonStatistics]()
            for stat in seasonStatistics {
                statistics[stat.playerId] = stat
            }
            
            self.seasonStatistics = statistics
        } catch {
            print("Failed to get season statistics: \(error)")
        }
    }
    
    private func loadPlayers() async {
        do {
            let allPlayers = try await sleeperClient.getPlayers()
            
            let validPositions = Set(["QB", "RB", "WR", "TE", "K", "DEF"])
            let filteredPlayers = allPlayers.values.filter { $0.active && validPositions.contains($0.position ?? "") }

            players = processPlayers(filteredPlayers)
            filterPlayers(players)
        } catch {
            print("Failed to get players: \(error)")
        }
    }
    
    private func processPlayers(_ players: [Player]) -> [Character: [Player]] {
        let sortedPlayers = players.sorted { player1, player2 in
            if player1.lastName == player2.lastName {
                return player1.firstName < player2.firstName
            } else {
                return player1.lastName < player2.lastName
            }
        }
        
        return Dictionary(grouping: sortedPlayers, by: { $0.lastName.first! })
    }
    
    private func filterPlayers(_ players: [Character: [Player]]) {
        filteredPlayers = players
        sectionHeaders = players.keys.sorted()
    }

    // MARK: - Table view data source

    override func numberOfSections(in tableView: UITableView) -> Int {
        sectionHeaders.count
    }

    override func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        let sectionHeader = sectionHeaders[section]
        return filteredPlayers[sectionHeader]!.count
    }

    override func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let cell = tableView.dequeueReusableCell(withIdentifier: cellId, for: indexPath) as! SubtitleRightDetailTableViewCell

        let sectionHeader = sectionHeaders[indexPath.section]
        let player = filteredPlayers[sectionHeader]![indexPath.row]
        let statistics = seasonStatistics[player.playerId]

        cell.mainLabel.text = "\(player.firstName) \(player.lastName)"
        cell.subtitleLabel.text = "\(player.position ?? "") - \(player.team ?? "")"

        if let points = statistics?.stats["pts_std"] {
            cell.rightLabel.text = String(points)
        }

        return cell
    }
    
    override func sectionIndexTitles(for tableView: UITableView) -> [String]? {
        sectionHeaders.map { $0.isLetter ? String($0) : "#" }
    }
    
    override func tableView(_ tableView: UITableView, didSelectRowAt indexPath: IndexPath) {
        let sectionHeader = sectionHeaders[indexPath.section]
        let player = filteredPlayers[sectionHeader]![indexPath.row]
        
        let viewController = PlayerDetailViewController(player: player, sleeperClient: sleeperClient)
        navigationController?.pushViewController(viewController, animated: true)
    }
    
    // MARK: - UISearchResultsUpdating
    
    func updateSearchResults(for searchController: UISearchController) {
        if let query = searchController.searchBar.text, !query.isEmpty {
            let playerQuery = players.values
                .flatMap { $0 }
                .filter { "\($0.firstName) \($0.lastName)".localizedCaseInsensitiveContains(query) }
            let processedPlayers = processPlayers(playerQuery)
            filterPlayers(processedPlayers)
        } else {
            filterPlayers(players)
        }
        
        tableView.reloadData()
    }
}
