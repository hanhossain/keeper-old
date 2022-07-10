//
//  StatisticSummaryViewController.swift
//  Keeper
//
//  Created by Han Hossain on 7/10/22.
//

import SigmaSwiftStatistics
import UIKit

class StatisticSummaryViewController: UITableViewController {
    private let aggregatedStatistics: [(stat: String, week: Int, value: Double)]
    private let summarizedStatistics: [(String, Double?)]
    private let cellId = "statisticCell"
    
    init(aggregatedStatistics: [(String, Int, Double)]) {
        self.aggregatedStatistics = aggregatedStatistics
        let values = aggregatedStatistics.map { $0.2 }
        
        summarizedStatistics = [
            ("Mean", Sigma.average(values)),
            ("Count", Double(values.count)),
            ("Minimum", Sigma.min(values)),
            ("Maximum", Sigma.max(values))
        ]
        
        super.init(style: .plain)
        
        title = "Summary"
        tabBarItem.image = UIImage(systemName: "sum")
        view.backgroundColor = .systemBackground
        tableView.register(RightDetailTableViewCell.self, forCellReuseIdentifier: cellId)
    }
    
    required init?(coder: NSCoder) {
        fatalError("init(coder:) has not been implemented")
    }
    
    override func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return summarizedStatistics.count
    }
    
    override func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let cell = tableView.dequeueReusableCell(withIdentifier: cellId, for: indexPath)
        let (name, value) = summarizedStatistics[indexPath.row]
        cell.textLabel?.text = name
        
        if let value = value {
            cell.detailTextLabel?.text = String(value)
        }
        
        return cell
    }
}
