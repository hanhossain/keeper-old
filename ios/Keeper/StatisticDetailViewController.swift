//
//  StatisticDetailViewController.swift
//  Keeper
//
//  Created by Han Hossain on 7/7/22.
//

import UIKit

class StatisticDetailViewController: UIViewController {
    private let aggregatedStatistics: [(stat: String, week: Int, value: Double?)]
    
    init(statName: String, aggregatedStatistics: [(String, Int, Double?)]) {
        self.aggregatedStatistics = aggregatedStatistics
        
        super.init(nibName: nil, bundle: nil)
        
        title = statName
        view.backgroundColor = .systemBackground
    }
    
    required init?(coder: NSCoder) {
        fatalError("init(coder:) has not been implemented")
    }
    
    override func viewDidLoad() {
        super.viewDidLoad()
        
        print(aggregatedStatistics)
    }
}
