//
//  NFLOffenseStatistics.swift
//  Keeper
//
//  Created by Han Hossain on 11/8/20.
//

import Foundation
import SwiftSoup

struct NFLOffenseStatistics {
    let passingStatistics: NFLPassingStatistics
    
    init?(from row: Element) {
        guard let passing = NFLPassingStatistics(from: row) else { return nil }
        passingStatistics = passing
    }
}
