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
    
    var name: String {
        return "\(firstName) \(lastName)"
    }
}
