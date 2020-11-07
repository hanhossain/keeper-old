//
//  SleeperPlayer.swift
//  Keeper
//
//  Created by Han Hossain on 11/6/20.
//

import Foundation

struct SleeperPlayer: Decodable {
    var firstName: String
    var lastName: String
    var fullName: String?
    var position: String?
    var positions: [String]?
    var active: Bool
    var team: String?
    var status: String?
    
    enum CodingKeys: String, CodingKey {
        case firstName = "first_name"
        case lastName = "last_name"
        case fullName = "full_name"
        case position = "position"
        case positions = "fantasy_positions"
        case active = "active"
        case team = "team"
        case status = "status"
    }
}
