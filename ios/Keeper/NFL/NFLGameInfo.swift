//
//  NFLGameInfo.swift
//  Keeper
//
//  Created by Han Hossain on 11/8/20.
//

import Foundation
import SwiftSoup

struct NFLGameInfo {
    let opponent: String
    let location: Location
    
    enum Location {
        case home
        case away
    }
    
    init?(from row: Element) {
        guard let opponentRaw = try? row.select(".playerOpponent").text() else { return nil }
        guard opponentRaw != "Bye" else { return nil }
        
        opponent = opponentRaw.trimmingCharacters(in: ["@"])
        location = opponentRaw.starts(with: "@") ? .away : .home
    }
}
