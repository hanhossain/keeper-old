//
//  NFLOffenseStatistics.swift
//  Keeper
//
//  Created by Han Hossain on 11/8/20.
//

import Foundation
import SwiftSoup

struct NFLOffenseStatistics {
    let passing: NFLPassingStatistics
    let rushing: NFLRushingStatistics
    let receiving: NFLReceivingStatistics
    
    init?(from row: Element) {
        guard let passing = NFLPassingStatistics(from: row) else { return nil }
        self.passing = passing
        
        guard let rushing = NFLRushingStatistics(from: row) else { return nil }
        self.rushing = rushing
        
        guard let receiving = NFLReceivingStatistics(from: row) else { return nil }
        self.receiving = receiving
    }
}
