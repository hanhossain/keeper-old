//
//  PlayersTableViewController.swift
//  Keeper
//
//  Created by Han Hossain on 11/7/20.
//

import UIKit

class PlayersTableViewController: UITableViewController {
    
    let cellId = "player"
    var players = [Character : [Player]]()
    var sections = [Character]()

    override func viewDidLoad() {
        super.viewDidLoad()
        
        let playerService = PlayerService()
        playerService.getPlayers { (players) in
            
            for player in players {
                var firstLetter = player.lastName.uppercased().first!
                if firstLetter.isNumber {
                    firstLetter = "#"
                }
                
                if self.sections.last != firstLetter {
                    self.sections.append(firstLetter)
                }
                
                if self.players[firstLetter] != nil {
                    self.players[firstLetter]!.append(player)
                } else {
                    self.players[firstLetter] = [player]
                }
            }
            
            DispatchQueue.main.async {
                self.tableView.reloadData()
            }
        }
    }

    // MARK: - Table view data source

    override func numberOfSections(in tableView: UITableView) -> Int {
        return sections.count
    }
    
    override func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return players[sections[section]]?.count ?? 0
    }

    override func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let cell = tableView.dequeueReusableCell(withIdentifier: cellId, for: indexPath)
        
        if let player = players[sections[indexPath.section]]?[indexPath.row] {
            cell.textLabel?.text = player.name
            cell.detailTextLabel?.text = player.positionAndTeam
        }

        return cell
    }
    
    override func tableView(_ tableView: UITableView, titleForHeaderInSection section: Int) -> String? {
        return String(sections[section])
    }
    
    override func sectionIndexTitles(for tableView: UITableView) -> [String]? {
        return sections.map { String($0) }
    }

    /*
    // MARK: - Navigation

    // In a storyboard-based application, you will often want to do a little preparation before navigation
    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        // Get the new view controller using segue.destination.
        // Pass the selected object to the new view controller.
    }
    */

}
