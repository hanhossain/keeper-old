//
//  PlayerTableViewController.swift
//  Keeper
//
//  Created by Han Hossain on 6/23/22.
//

import UIKit

class PlayerTableViewController: UITableViewController {
    private var sections = [Character]()
    private var players = [Character: [Player]]()
    private var seasonStatistics = [String: SeasonStatistics]()
    
    private let cellId = "playerCell"

    override func viewDidLoad() {
        super.viewDidLoad()
        tableView.register(SubtitleRightDetailTableViewCell.self, forCellReuseIdentifier: cellId)
        title = "Players"
        
        Task {
            async let players: () = getPlayers()
            async let seasonStatistics: () = getSeasonStatistics()
            
            await players
            await seasonStatistics
            
            tableView.reloadData()
        }
    }
    
    func getSeasonStatistics() async {
        do {
            let url = URL(string: "https://api.sleeper.com/stats/nfl/2021?season_type=regular")!
            let (data, _) = try await URLSession.shared.data(from: url)
            print("Received season statistics")
            
            let decoder = JSONDecoder()
            decoder.keyDecodingStrategy = .convertFromSnakeCase
            
            if let seasonStatistics = try? decoder.decode([SeasonStatistics].self, from: data) {
                var statistics = [String: SeasonStatistics]()
                for stat in seasonStatistics {
                    statistics[stat.playerId] = stat
                }
                
                self.seasonStatistics = statistics
            }
        } catch {
            print("Failed to get season statistics: \(error)")
        }
    }
    
    func getPlayers() async {
        do {
            let url = URL(string: "https://api.sleeper.app/v1/players/nfl")!
            let (data, _) = try await URLSession.shared.data(from: url)
            print("Received players")
            
            let decoder = JSONDecoder()
            decoder.keyDecodingStrategy = .convertFromSnakeCase
            
            if let allPlayers = try? decoder.decode([String: Player].self, from: data) {
                let validPositions = Set(["QB", "RB", "WR", "TE", "K", "DEF"])
                let filteredPlayers = allPlayers.values
                    .filter { $0.active && validPositions.contains($0.position ?? "") }
                    .sorted { player1, player2 in
                        if player1.lastName == player2.lastName {
                            return player1.firstName < player2.firstName
                        } else {
                            return player1.lastName < player2.lastName
                        }
                    }

                players = Dictionary(grouping: filteredPlayers, by: { $0.lastName.first! })
                sections = players.keys.sorted()
            }
        } catch {
            print("Failed to get players: \(error)")
        }
    }

    // MARK: - Table view data source

    override func numberOfSections(in tableView: UITableView) -> Int {
        sections.count
    }

    override func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        players[sections[section]]!.count
    }

    override func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let cell = tableView.dequeueReusableCell(withIdentifier: cellId, for: indexPath) as! SubtitleRightDetailTableViewCell

        let section = sections[indexPath.section]
        let player = players[section]![indexPath.row]
        let statistics = seasonStatistics[player.playerId]

        cell.mainLabel.text = "\(player.firstName) \(player.lastName)"
        cell.subtitleLabel.text = "\(player.position ?? "") - \(player.team ?? "")"

        if let points = statistics?.stats["pts_std"] {
            cell.rightLabel.text = String(points)
        }

        return cell
    }
    
    override func sectionIndexTitles(for tableView: UITableView) -> [String]? {
        sections.map { $0.isLetter ? String($0) : "#" }
    }
}
