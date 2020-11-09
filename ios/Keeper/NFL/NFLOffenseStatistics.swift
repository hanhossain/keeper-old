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
    let returning: NFLReturningStatistics
    
    init(from row: Element) {
        passing = NFLPassingStatistics(from: row)
        rushing = NFLRushingStatistics(from: row)
        receiving = NFLReceivingStatistics(from: row)
        returning = NFLReturningStatistics(from: row)
    }
}
