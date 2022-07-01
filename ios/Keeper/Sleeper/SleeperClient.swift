//
//  SleeperClient.swift
//  Keeper
//
//  Created by Han Hossain on 6/30/22.
//

import Foundation
import UIKit

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
    
    func getAvatar(playerId: String) async throws -> UIImage? {
        let uri = playerId.first!.isLetter
            ? "https://sleepercdn.com/images/team_logos/nfl/\(playerId.lowercased()).png"
            : "https://sleepercdn.com/content/nfl/players/\(playerId).jpg"
        
        let url = URL(string: uri)!
        let (data, response) = try await URLSession.shared.data(from: url)
        
        guard
            let httpResponse = response as? HTTPURLResponse,
            httpResponse.statusCode == 200 else {
            return nil
        }
        
        return UIImage(data: data)
    }
}
