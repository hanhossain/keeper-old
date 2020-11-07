//
//  SleeperService.swift
//  Keeper
//
//  Created by Han Hossain on 11/6/20.
//

import Foundation

class SleeperClient {
    func getPlayers(completion: @escaping ([String : SleeperPlayer]) -> ()) {
        URLSession.shared.dataTask(with: URL(string: "https://api.sleeper.app/v1/players/nfl")!) { (data, res, _) in
            guard let data = data else { return }
            
            do {
                let result = try JSONDecoder().decode([String : SleeperPlayer].self, from: data)
                completion(result)
            } catch {
                print(error.localizedDescription)
            }
        }.resume()
    }
}
