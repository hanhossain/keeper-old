//
//  StatisticDetailViewController.swift
//  Keeper
//
//  Created by Han Hossain on 7/7/22.
//

import UIKit

class StatisticDetailViewController: UIViewController {
    
    init(statName: String) {
        super.init(nibName: nil, bundle: nil)
        
        title = statName
        view.backgroundColor = .systemBackground
    }
    
    required init?(coder: NSCoder) {
        fatalError("init(coder:) has not been implemented")
    }
}
