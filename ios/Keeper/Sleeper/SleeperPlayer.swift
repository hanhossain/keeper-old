//
//  SleeperPlayer.swift
//  Keeper
//
//  Created by Han Hossain on 11/6/20.
//

import Foundation

struct SleeperPlayer: Decodable {
    let firstName: String
    let lastName: String
    let fullName: String?
    let position: String?
    let active: Bool
    let team: String?
    let status: String?
    let playerId: String
    
    enum CodingKeys: String, CodingKey {
        case firstName = "first_name"
        case lastName = "last_name"
        case fullName = "full_name"
        case position = "position"
        case active = "active"
        case team = "team"
        case status = "status"
        case playerId = "player_id"
    }
    
    init(from: ManagedSleeperPlayer) {
        firstName = from.firstName!
        lastName = from.lastName!
        fullName = from.fullName
        position = from.position
        active = from.active
        team = from.team
        status = from.status
        playerId = from.playerId!
    }
}
