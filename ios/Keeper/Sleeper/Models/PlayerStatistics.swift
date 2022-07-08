//
//  PlayerStatistics.swift
//  Keeper
//
//  Created by Han Hossain on 7/7/22.
//

import Foundation

struct PlayerStatistics: Decodable {
    let week: Int
    let team: String
    let opponent: String
    let date: String
    let stats: [String: Double]
}
