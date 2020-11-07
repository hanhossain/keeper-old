//
//  PlayerService.swift
//  Keeper
//
//  Created by Han Hossain on 11/7/20.
//

import Foundation

class PlayerService {
    let sleeper = SleeperClient()
    
    func getPlayers(completion: @escaping ([Player]) -> ()) {
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
            
            completion(players)
        }
    }
}
