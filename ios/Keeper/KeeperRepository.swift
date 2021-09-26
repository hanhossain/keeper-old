//
//  KeeperRepository.swift
//  Keeper
//
//  Created by Han Hossain on 9/25/21.
//

import CoreData
import UIKit

class KeeperRepository {
    let validPositions: Set = ["QB", "RB", "WR", "TE", "K", "DEF"]
    
    let appDelegate: AppDelegate
    let context: NSManagedObjectContext
    
    
    init(delegate: AppDelegate) {
        appDelegate = delegate
        context = appDelegate.persistentContainer.viewContext
    }
    
    func upsertSleeperPlayers(players: [String : SleeperPlayer]) {
        for player in players.values {
            let fetchRequest: NSFetchRequest = ManagedSleeperPlayer.fetchRequest()
            fetchRequest.fetchLimit = 1
            fetchRequest.predicate = NSPredicate(format: "playerId == %@", player.playerId)
            let results = try! context.fetch(fetchRequest)
            
            let playerEntity = results.first ?? ManagedSleeperPlayer(context: context)
            playerEntity.active = player.active
            playerEntity.firstName = player.firstName
            playerEntity.fullName = player.fullName
            playerEntity.lastName = player.lastName
            playerEntity.position = player.position
            playerEntity.status = player.status
            playerEntity.team = player.team
            playerEntity.playerId = player.playerId
        }
        
        appDelegate.saveContext()
    }
    
    func getSleeperPlayers() -> [SleeperPlayer] {
        let fetchRequest: NSFetchRequest = ManagedSleeperPlayer.fetchRequest()
        fetchRequest.predicate = NSPredicate(format: "active == true")
        let players = try! context.fetch(fetchRequest)
            .filter { validPositions.contains($0.position ?? "") }
            .map { SleeperPlayer(from: $0) }
            .sorted { (left, right) -> Bool in
                if left.lastName == right.lastName {
                    return left.firstName < right.firstName
                }

                return left.lastName < right.lastName
            }
        return players
    }
    
    
}
