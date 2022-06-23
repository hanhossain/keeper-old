//
//  ContentView.swift
//  Keeper
//
//  Created by Han Hossain on 6/23/22.
//

import SwiftUI

struct Player: Decodable {
    let playerId: String
    let firstName: String
    let lastName: String
    let position: String?
    let team: String?
    let active: Bool
}

struct ContentView: View {
    @State private var players = [Player]()
    
    var body: some View {
        NavigationView {
            List(players, id: \.playerId) { player in
                VStack(alignment: .leading) {
                    Text("\(player.firstName) \(player.lastName)")
                    Text("\(player.position ?? "") - \(player.team ?? "")")
                        .font(.footnote)
                }
            }
            .navigationTitle("Players")
            .task {
                await getPlayers()
            }
        }
    }
    
    func getPlayers() async {
        do {
            let url = URL(string: "https://api.sleeper.app/v1/players/nfl")!
            let (data, _) = try await URLSession.shared.data(from: url)
            let decoder = JSONDecoder()
            decoder.keyDecodingStrategy = .convertFromSnakeCase
            if let allPlayers = try? decoder.decode([String : Player].self, from: data) {
                let validPositions = Set(["QB", "RB", "WR", "TE", "K", "DEF"])
                players = allPlayers.values
                    .filter { $0.active && validPositions.contains($0.position ?? "") }
                    .sorted { player1, player2 in
                        player1.lastName < player2.lastName
                    }
            }
        } catch {
            print("oops, something broke: \(error)")
        }
    }
}

struct ContentView_Previews: PreviewProvider {
    static var previews: some View {
        ContentView()
    }
}
