//
//  SleeperNFLState.swift
//  Keeper
//
//  Created by Han Hossain on 9/25/21.
//

import Foundation

struct SleeperNFLState: Decodable {
    let week: Int
    let seasonType: String
    let seasonStartDate: String
    let season: Int
    let previousSeason: Int
    let leg: Int
    let leagueSeason: Int
    let leagueCreateSeason: Int
    let displayWeek: Int
    
    enum CodingKeys: String, CodingKey {
        case week = "week"
        case seasonType = "season_type"
        case seasonStartDate = "season_start_date"
        case season = "season"
        case previousSeason = "previous_season"
        case leg = "leg"
        case leagueSeason = "league_season"
        case leagueCreateSeason = "league_create_season"
        case displayWeek = "display_week"
    }
    
    init(from decoder: Decoder) throws {
        let container = try decoder.container(keyedBy: CodingKeys.self)
        week = try container.decode(Int.self, forKey: .week)
        seasonType = try container.decode(String.self, forKey: .seasonType)
        seasonStartDate = try container.decode(String.self, forKey: .seasonStartDate)
        season = Int(try container.decode(String.self, forKey: .season))!
        previousSeason = Int(try container.decode(String.self, forKey: .previousSeason))!
        leg = try container.decode(Int.self, forKey: .leg)
        leagueSeason = Int(try container.decode(String.self, forKey: .leagueSeason))!
        leagueCreateSeason = Int(try container.decode(String.self, forKey: .leagueCreateSeason))!
        displayWeek = try container.decode(Int.self, forKey: .displayWeek)
    }
}
