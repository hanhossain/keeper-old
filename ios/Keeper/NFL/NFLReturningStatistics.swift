//
//  NFLReturningStatistics.swift
//  Keeper
//
//  Created by Han Hossain on 11/8/20.
//

import Foundation
import SwiftSoup

struct NFLReturningStatistics {
    let touchdowns: Int?
    
    init(from row: Element) {
        if let touchdowns = try? row.select(".statId-28").text() {
            self.touchdowns = Int(touchdowns)
        } else {
            touchdowns = nil
        }
    }
}
