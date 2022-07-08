//
//  PlayerDetailViewController.swift
//  Keeper
//
//  Created by Han Hossain on 7/1/22.
//

import UIKit

class PlayerDetailViewController: UIViewController, UITableViewDataSource {
    private let player: Player
    private let sleeperClient: SleeperClient
    
    private let cellId = "playerDetailCell"
    
    private var tableView: UITableView!
    
    init(player: Player, sleeperClient: SleeperClient, seasonStatistics: SeasonStatistics) {
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
        
        tableView = UITableView()
        tableView.backgroundColor = .systemBackground
        tableView.dataSource = self
        tableView.register(RightDetailTableViewCell.self, forCellReuseIdentifier: cellId)
        
        let metadataStackView = UIStackView()
        metadataStackView.distribution = .fillProportionally
        
        let positionTeamLabel = UILabel()
        positionTeamLabel.text = "\(player.position ?? "") - \(player.team ?? "")"
        metadataStackView.addArrangedSubview(positionTeamLabel)
        
        let stackView = UIStackView(arrangedSubviews: [metadataStackView, tableView])
        stackView.axis = .vertical
        
        view.addSubview(stackView)
        stackView.pin(to: view.safeAreaLayoutGuide)
        
        Task {
            let avatar = try! await sleeperClient.getAvatar(playerId: player.playerId)
                ?? UIImage(systemName: "person.fill.questionmark", withConfiguration: UIImage.SymbolConfiguration(pointSize: 71))
            let avatarView = UIImageView(image: avatar)
            avatarView.tintColor = .systemGray
            avatarView.contentMode = .scaleAspectFit

            metadataStackView.insertArrangedSubview(avatarView, at: 0)
        }
    }
    
    // MARK: - UITableViewDataSource
    
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return 1
    }
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let cell = tableView.dequeueReusableCell(withIdentifier: cellId, for: indexPath)
        cell.textLabel?.text = "Hello world"
        
        return cell
    }
}
