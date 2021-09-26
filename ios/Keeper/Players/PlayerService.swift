//
//  PlayerService.swift
//  Keeper
//
//  Created by Han Hossain on 11/7/20.
//

import Foundation

class PlayerService {
    let sleeper = SleeperClient()
    let nfl = NFLClient()
    let appDelegate: AppDelegate
    
    var players: [SleeperPlayer]?
    
    init(delegate: AppDelegate) {
        appDelegate = delegate
    }
    
    func getPlayers(completion: @escaping ([SleeperPlayer]) -> ()) {
        if let players = players {
            completion(players)
            return
        }
        
        sleeper.getPlayers { (players) in
            let keeperRepository = KeeperRepository(delegate: self.appDelegate)
            keeperRepository.upsertSleeperPlayers(players: players)
            let players = keeperRepository.getSleeperPlayers()
            
            self.players = players
            completion(players)
        }
    }
    
    func getStatistics() {
        guard let url = URL(string: "https://fantasy.nfl.com/research/players?position=1&statCategory=stats&statSeason=2020&statType=weekStats&statWeek=5")
            else { return }
        nfl.getHTML(url: url) { (raw) in
//            print(raw)
        }
    }
}
