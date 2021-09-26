//
//  Configuration.swift
//  Keeper
//
//  Created by Han Hossain on 9/25/21.
//

import Foundation

class Configuration {
    private static let dateFormatter = ISO8601DateFormatter()
    
    static var sleeperDate: Date? {
        get {
            guard let date = UserDefaults.standard.string(forKey: "sleeperDate") else {
                return nil
            }
            return dateFormatter.date(from: date)
        }
        
        set {
            guard let date = newValue else {
                return
            }
            let rawDate = dateFormatter.string(from: date)
            UserDefaults.standard.set(rawDate, forKey: "sleeperDate")
        }
    }
}
