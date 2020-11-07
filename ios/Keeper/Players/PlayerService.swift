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
            let players = players.values
                .filter({ $0.active })
                .map({ Player(firstName: $0.firstName, lastName: $0.lastName) })
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
