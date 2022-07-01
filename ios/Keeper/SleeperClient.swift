//
//  SleeperClient.swift
//  Keeper
//
//  Created by Han Hossain on 6/30/22.
//

import Foundation

class SleeperClient {
    private let decoder: JSONDecoder = {
        let decoder = JSONDecoder()
        decoder.keyDecodingStrategy = .convertFromSnakeCase
        return decoder
    }()
    
    func getPlayers() async throws -> [String: Player] {
        let url = URL(string: "https://api.sleeper.app/v1/players/nfl")!
        let (data, _) = try await URLSession.shared.data(from: url)
        
        return try decoder.decode([String: Player].self, from: data)
    }
    
    func getSeasonStatistics() async throws -> [SeasonStatistics] {
        let url = URL(string: "https://api.sleeper.com/stats/nfl/2021?season_type=regular")!
        let (data, _) = try await URLSession.shared.data(from: url)
        
        return try decoder.decode([SeasonStatistics].self, from: data)
    }
}
