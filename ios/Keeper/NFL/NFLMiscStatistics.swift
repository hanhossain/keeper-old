//
//  NFLMiscStatistics.swift
//  Keeper
//
//  Created by Han Hossain on 11/8/20.
//

import Foundation
import SwiftSoup

struct NFLMiscStatistics {
    let fumbledTouchdowns: Int?
    let twoPointConversions: Int?
    
    init(from row: Element) {
        if let fumbledTouchdowns = try? row.select(".statId-29").text() {
            self.fumbledTouchdowns = Int(fumbledTouchdowns)
        } else {
            fumbledTouchdowns = nil
        }
        
        if let twoPointConversions = try? row.select(".statId-32").text() {
            self.twoPointConversions = Int(twoPointConversions)
        } else {
            twoPointConversions = nil
        }
    }
}
