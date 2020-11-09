//
//  PlayerService.swift
//  Keeper
//
//  Created by Han Hossain on 11/7/20.
//

import Foundation

class PlayerService {
    let sleeper = SleeperClient()
    let nfl = NFLClient()
    
    var players: [Player]?
    
    func getPlayers(completion: @escaping ([Player]) -> ()) {
        if let players = players {
            completion(players)
            return
        }
        
        sleeper.getPlayers { (players) in
            let validPositions: Set = ["QB", "RB", "WR", "TE", "K", "DEF"]
            
            let players = players.values
                .filter({ $0.active && validPositions.contains($0.position ?? "")})
                .map({ Player(firstName: $0.firstName, lastName: $0.lastName, position: $0.position!, team: $0.team) })
                .sorted { (left, right) -> Bool in
                    if left.lastName == right.lastName {
                        return left.firstName < right.lastName
                    }

                    return left.lastName < right.lastName
                }
            
            self.players = players
            completion(players)
        }
    }
    
    func getStatistics() {
        guard let url = URL(string: "https://fantasy.nfl.com/research/players?position=1&statCategory=stats&statSeason=2020&statType=weekStats&statWeek=5")
            else { return }
        nfl.getHTML(url: url) { (raw) in
//            print(raw)
        }
    }
}
