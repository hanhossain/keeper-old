//
//  NFLPlayer.swift
//  Keeper
//
//  Created by Han Hossain on 11/8/20.
//

import Foundation
import SwiftSoup

struct NFLPlayer {
    let id: String
    let name: String
    let position: String
    let team: String?
    let gameInfo: NFLGameInfo?
    let fantasyPoints: Double
    let statistics: NFLStatistics
    
    init?(from row: Element) {
        guard let rawId = try? row.classNames().first?.split(separator: "-").last else { return nil }
        id = String(rawId)
        
        guard let name = try? row.select(".playerName").text() else { return nil }
        self.name = name
        
        guard let positionAndTeam = try? row.select("em").text().split(separator: "-") else { return nil }
        guard let position = positionAndTeam.first?.trimmingCharacters(in: .whitespaces) else { return nil }
        self.position = position
        team = positionAndTeam.last?.trimmingCharacters(in: .whitespaces)

        gameInfo = NFLGameInfo(from: row)

        
        if let fantasyPointsRaw = try? row.select(".playerTotal").text() {
            fantasyPoints = Double(fantasyPointsRaw) ?? 0
        } else {
            fantasyPoints = 0
        }
        
        guard let statistics = NFLStatistics(from: row, position: position) else { return nil }
        self.statistics = statistics
    }
}
