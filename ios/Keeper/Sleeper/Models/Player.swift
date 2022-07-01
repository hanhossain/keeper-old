//
//  Player.swift
//  Keeper
//
//  Created by Han Hossain on 6/23/22.
//

import Foundation

struct Player: Decodable {
    let playerId: String
    let firstName: String
    let lastName: String
    let position: String?
    let team: String?
    let active: Bool
}
