//
//  PlayerTableViewController.swift
//  Keeper
//
//  Created by Han Hossain on 6/23/22.
//

import UIKit

class PlayerTableViewController: UITableViewController {
    private var players = [Player]()

    override func viewDidLoad() {
        super.viewDidLoad()
        title = "Players"
        
        Task {
            await getPlayers()
        }
    }
    
    func getPlayers() async {
        do {
            let url = URL(string: "https://api.sleeper.app/v1/players/nfl")!
            let (data, _) = try await URLSession.shared.data(from: url)
            let decoder = JSONDecoder()
            decoder.keyDecodingStrategy = .convertFromSnakeCase
            if let players = try? decoder.decode([String : Player].self, from: data) {
                let validPositions = Set(["QB", "RB", "WR", "TE", "K", "DEF"])
                self.players = players.values
                    .filter { $0.active && validPositions.contains($0.position ?? "") }
                    .sorted { player1, player2 in
                        if player1.lastName == player2.lastName {
                            return player1.firstName < player2.firstName
                        } else {
                            return player1.lastName < player2.lastName
                        }
                    }
                tableView.reloadData()
            }
        } catch {
            print("Failed to get players: \(error)")
        }
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
        cell.textLabel?.text = "\(player.firstName) \(player.lastName)"
        cell.detailTextLabel?.text = "\(player.position ?? "") - \(player.team ?? "")"

        return cell
    }
}
