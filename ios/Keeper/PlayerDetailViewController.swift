//
//  PlayerDetailViewController.swift
//  Keeper
//
//  Created by Han Hossain on 7/1/22.
//

import UIKit

class PlayerDetailViewController: UIViewController, UITableViewDataSource, UITableViewDelegate {
    private let player: Player
    private let sleeperClient: SleeperClient
    private let seasonStatistics: SeasonStatistics
    private let statisticsKeys: [String]
    
    private let cellId = "playerDetailCell"
    
    private var tableView: UITableView!
    
    init(player: Player, sleeperClient: SleeperClient, seasonStatistics: SeasonStatistics) {
        self.player = player
        self.sleeperClient = sleeperClient
        self.seasonStatistics = seasonStatistics
        statisticsKeys = seasonStatistics.stats.keys.sorted()
        
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
        tableView.delegate = self
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
        return statisticsKeys.count
    }
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let statKey = statisticsKeys[indexPath.row]
        let statValue = seasonStatistics.stats[statKey]!
        
        let cell = tableView.dequeueReusableCell(withIdentifier: cellId, for: indexPath)
        cell.textLabel?.text = statKey
        cell.detailTextLabel?.text = String(statValue)
        
        return cell
    }
    
    // MARK: - UITableViewDelegate
    
    func tableView(_ tableView: UITableView, didSelectRowAt indexPath: IndexPath) {
        let statKey = statisticsKeys[indexPath.row]
        let vc = StatisticDetailViewController(statName: statKey)
        navigationController?.pushViewController(vc, animated: true)
    }
}
