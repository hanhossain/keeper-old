//
//  PlayerDetailViewController.swift
//  Keeper
//
//  Created by Han Hossain on 7/1/22.
//

import UIKit

class PlayerDetailViewController: UIViewController {
    private let player: Player
    private let sleeperClient: SleeperClient
    
    init(player: Player, sleeperClient: SleeperClient) {
        self.player = player
        self.sleeperClient = sleeperClient
        super.init(nibName: nil, bundle: nil)
    }
    
    required init?(coder: NSCoder) {
        fatalError("init(coder:) has not been implemented")
    }
    
    override func viewDidLoad() {
        super.viewDidLoad()
        view.backgroundColor = .systemBackground
        title = "\(player.firstName) \(player.lastName)"
        
        let stackView = UIStackView()
        stackView.axis = .vertical
        
        view.addSubview(stackView)
        stackView.pin(to: view.safeAreaLayoutGuide)
        
        Task {
            let avatar = try! await sleeperClient.getAvatar(playerId: player.playerId)
                ?? UIImage(systemName: "person.fill.questionmark", withConfiguration: UIImage.SymbolConfiguration(pointSize: 71))
            let avatarView = UIImageView(image: avatar)
            avatarView.tintColor = .systemGray
            avatarView.contentMode = .scaleAspectFit
            
            stackView.addArrangedSubview(avatarView)
        }
    }
}
