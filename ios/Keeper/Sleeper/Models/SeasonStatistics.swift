//
//  SeasonStatistics.swift
//  Keeper
//
//  Created by Han Hossain on 6/23/22.
//

import Foundation

struct SeasonStatistics: Decodable {
    let playerId: String
    let season: String
    let stats: [String: Double]
}
