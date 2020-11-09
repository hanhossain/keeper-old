//
//  NFLStatistics.swift
//  Keeper
//
//  Created by Han Hossain on 11/8/20.
//

import Foundation
import SwiftSoup

enum NFLStatistics {
    case offense(NFLOffenseStatistics)
    
    init?(from row: Element, position: String) {
        switch position {
        case "QB", "RB", "WR", "TE":
            self = .offense(NFLOffenseStatistics(from: row))
        default:
            return nil
        }
    }
}
