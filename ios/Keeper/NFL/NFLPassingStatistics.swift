//
//  NFLPassingStatistics.swift
//  Keeper
//
//  Created by Han Hossain on 11/8/20.
//

import Foundation
import SwiftSoup

struct NFLPassingStatistics {
    let yards: Int?
    let touchdowns: Int?
    let interceptions: Int?
    
    init(from row: Element) {
        if let yards = try? row.select(".statId-5").text() {
            self.yards = Int(yards)
        } else {
            yards = nil
        }
        
        if let touchdowns = try? row.select(".statId-6").text() {
            self.touchdowns = Int(touchdowns)
        } else {
            touchdowns = nil
        }
        
        if let interceptions = try? row.select(".statId-7").text() {
            self.interceptions = Int(interceptions)
        } else {
            interceptions = nil
        }
    }
}
