//
//  NFLReceivingStatistics.swift
//  Keeper
//
//  Created by Han Hossain on 11/8/20.
//

import Foundation
import SwiftSoup

struct NFLReceivingStatistics {
    let receptions: Int?
    let yards: Int?
    let touchdowns: Int?
    
    init?(from row: Element) {
        if let receptions = try? row.select(".statId-20").text() {
            self.receptions = Int(receptions)
        } else {
            receptions = nil
        }
        
        if let yards = try? row.select(".statId-21").text() {
            self.yards = Int(yards)
        } else {
            yards = nil
        }
        
        if let touchdowns = try? row.select(".statId-22").text() {
            self.touchdowns = Int(touchdowns)
        } else {
            touchdowns = nil
        }
    }
}
