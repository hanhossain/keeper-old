//
//  PlayerTableViewController.swift
//  Keeper
//
//  Created by Han Hossain on 6/23/22.
//

import UIKit

class PlayerTableViewController: UITableViewController {
    private var players = [Player]()
    private var seasonStatistics = [String: SeasonStatistics]()

    override func viewDidLoad() {
        super.viewDidLoad()
        title = "Players"
        
        Task {
            async let players = getPlayers()
            async let seasonStatistics = getSeasonStatistics()
            
            self.players = await players
            self.seasonStatistics = await seasonStatistics
            
            tableView.reloadData()
        }
    }
    
    func getSeasonStatistics() async -> [String: SeasonStatistics] {
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
                
                return statistics
            }
        } catch {
            print("Failed to get season statistics: \(error)")
        }
        
        return [:]
    }
    
    func getPlayers() async -> [Player] {
        do {
            let url = URL(string: "https://api.sleeper.app/v1/players/nfl")!
            let (data, _) = try await URLSession.shared.data(from: url)
            print("Received players")
            
            let decoder = JSONDecoder()
            decoder.keyDecodingStrategy = .convertFromSnakeCase
            
            if let players = try? decoder.decode([String: Player].self, from: data) {
                let validPositions = Set(["QB", "RB", "WR", "TE", "K", "DEF"])
                return players.values
                    .filter { $0.active && validPositions.contains($0.position ?? "") }
                    .sorted { player1, player2 in
                        if player1.lastName == player2.lastName {
                            return player1.firstName < player2.firstName
                        } else {
                            return player1.lastName < player2.lastName
                        }
                    }
                
            }
        } catch {
            print("Failed to get players: \(error)")
        }
        
        return []
    }

    // MARK: - Table view data source

    override func numberOfSections(in tableView: UITableView) -> Int {
        return 1
    }

    override func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return players.count
    }

    override func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let cell = tableView.dequeueReusableCell(withIdentifier: "playerCell", for: indexPath)

        let player = players[indexPath.row]
        let statistics = seasonStatistics[player.playerId]
        cell.textLabel?.text = "\(player.firstName) \(player.lastName)"
        
        if let points = statistics?.stats["pts_std"] {
            cell.detailTextLabel?.text  = "\(player.position ?? "") - \(player.team ?? "") - \(points)"
        } else {
            cell.detailTextLabel?.text  = "\(player.position ?? "") - \(player.team ?? "")"
        }

        return cell
    }
}
