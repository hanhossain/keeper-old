//
//  PlayersTableViewController.swift
//  Keeper
//
//  Created by Han Hossain on 11/7/20.
//

import UIKit

class PlayersTableViewController: UITableViewController {
    
    let cellId = "player"
    let searchController = UISearchController(searchResultsController: nil)
    let playerService = PlayerService()
    
    var players = [Character : [Player]]()
    var filteredPlayers = [Character : [Player]]()
    
    var sections = [Character]()
    var filteredSections = [Character]()
    
    var isSearchBarEmpty: Bool {
        return searchController.searchBar.text?.isEmpty ?? true
    }
    
    var isFiltering: Bool {
        return searchController.isActive && !isSearchBarEmpty
    }
    
    override func viewDidLoad() {
        super.viewDidLoad()
        
        // setup search controller
        searchController.searchResultsUpdater = self
        searchController.obscuresBackgroundDuringPresentation = false
        searchController.searchBar.placeholder = "Search Players"
        navigationItem.searchController = searchController
        definesPresentationContext = true
        
        playerService.getPlayers { (players) in
            
            (self.players, self.sections) = self.getPlayersAndSections(from: players)
            
            DispatchQueue.main.async {
                self.tableView.reloadData()
            }
        }
    }
    
    func getPlayersAndSections(from originalPlayers: [Player]) -> (players: [Character : [Player]], sections: [Character]) {
        var players = [Character : [Player]]()
        var sections = [Character]()
        
        for player in originalPlayers {
            var firstLetter = player.lastName.uppercased().first!
            if firstLetter.isNumber {
                firstLetter = "#"
            }
            
            if sections.last != firstLetter {
                sections.append(firstLetter)
            }
            
            if players[firstLetter] != nil {
                players[firstLetter]!.append(player)
            } else {
                players[firstLetter] = [player]
            }
        }
        
        return (players, sections)
    }
    
    func filterPlayers(_ query: String) {
        let normalizedQuery = query.lowercased()
        
        playerService.getPlayers { (players: [Player]) in
            let filtered = players.filter { (player) -> Bool in
                return player.name.lowercased().contains(normalizedQuery)
            }
            
            (self.filteredPlayers, self.filteredSections) = self.getPlayersAndSections(from: filtered)
            
            DispatchQueue.main.async {
                self.tableView.reloadData()
            }
        }
    }

    // MARK: - Table view data source

    override func numberOfSections(in tableView: UITableView) -> Int {
        return isFiltering ? filteredSections.count : sections.count
    }
    
    override func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        let players = isFiltering ? filteredPlayers : self.players
        let sections = isFiltering ? filteredSections : self.sections
        return players[sections[section]]?.count ?? 0
    }

    override func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let cell = tableView.dequeueReusableCell(withIdentifier: cellId, for: indexPath)
        
        let players = isFiltering ? filteredPlayers : self.players
        let sections = isFiltering ? filteredSections : self.sections
        
        if let player = players[sections[indexPath.section]]?[indexPath.row] {
            cell.textLabel?.text = player.name
            cell.detailTextLabel?.text = player.positionAndTeam
        }

        return cell
    }
    
    override func tableView(_ tableView: UITableView, titleForHeaderInSection section: Int) -> String? {
        let sections = isFiltering ? filteredSections : self.sections
        return String(sections[section])
    }
    
    override func sectionIndexTitles(for tableView: UITableView) -> [String]? {
        let sections = isFiltering ? filteredSections : self.sections
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

extension PlayersTableViewController: UISearchResultsUpdating {
    func updateSearchResults(for searchController: UISearchController) {
        filterPlayers(searchController.searchBar.text!)
    }
}
