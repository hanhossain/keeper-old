//
//  NFLClient.swift
//  Keeper
//
//  Created by Han Hossain on 11/8/20.
//

import Foundation
import SwiftSoup

class NFLClient {
    func getHTML(url: URL, completion: @escaping (String) -> ()) {
        // https://fantasy.nfl.com/research/players?position=1&statCategory=stats&statSeason=2020&statType=weekStats&statWeek=5
        URLSession.shared.dataTask(with: url) { (data, _, _) in
            guard let data = data else { return }
            
            guard let string = String(data: data, encoding: .utf8) else { return }
            if let players = self.parse(string) {
                if let player = players.first {
                    print(player.name)
                    print(player.statistics)
                }
                completion(string)
            }
        }.resume()
    }
    
    private func parse(_ html: String) -> [NFLPlayer]? {
        guard let doc = try? SwiftSoup.parse(html) else { return nil }
        guard let table = try? doc.select("table").first() else { return nil }
        guard let tableBody = try? table.select("tbody").first() else { return nil }
        let rows = tableBody.children().array()
        return rows.compactMap { NFLPlayer(from: $0) }
    }
}
