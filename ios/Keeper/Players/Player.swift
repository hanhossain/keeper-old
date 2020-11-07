//
//  Player.swift
//  Keeper
//
//  Created by Han Hossain on 11/7/20.
//

import Foundation

struct Player {
    let firstName: String
    let lastName: String
    let position: String
    let team: String?
    
    var name: String {
        return "\(firstName) \(lastName)"
    }
}
