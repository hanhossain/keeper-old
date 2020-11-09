//
//  NFLFumbleStatistics.swift
//  Keeper
//
//  Created by Han Hossain on 11/8/20.
//

import Foundation
import SwiftSoup

struct NFLFumbleStatistics {
    let lost: Int?
    
    init(from row: Element) {
        if let lost = try? row.select(".statId-30").text() {
            self.lost = Int(lost)
        } else {
            lost = nil
        }
    }
}
