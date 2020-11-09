//
//  NFLRushingStatistics.swift
//  Keeper
//
//  Created by Han Hossain on 11/8/20.
//

import Foundation
import SwiftSoup

struct NFLRushingStatistics {
    let yards: Int?
    let touchdowns: Int?
    
    init(from row: Element) {
        if let yards = try? row.select(".statId-14").text() {
            self.yards = Int(yards)
        } else {
            yards = nil
        }
        
        if let touchdowns = try? row.select(".statId-15").text() {
            self.touchdowns = Int(touchdowns)
        } else {
            touchdowns = nil
        }
    }
}
