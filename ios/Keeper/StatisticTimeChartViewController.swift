//
//  StatisticTimeChartViewController.swift
//  Keeper
//
//  Created by Han Hossain on 7/7/22.
//

import Charts
import UIKit

class StatisticTimeChartViewController: UIViewController {
    private let aggregatedStatistics: [(stat: String, week: Int, value: Double?)]
    
    init(aggregatedStatistics: [(String, Int, Double?)]) {
        self.aggregatedStatistics = aggregatedStatistics.sorted { $0.1 < $1.1 }
        
        super.init(nibName: nil, bundle: nil)
        
        title = "Time chart"
        view.backgroundColor = .systemBackground
    }
    
    required init?(coder: NSCoder) {
        fatalError("init(coder:) has not been implemented")
    }
    
    override func viewDidLoad() {
        super.viewDidLoad()
        
        let entries = aggregatedStatistics.map { ChartDataEntry(x: Double($0.week), y: $0.value ?? 0) }
        let dataSet = LineChartDataSet(entries: entries, label: "2021")
        
        let chartView = LineChartView()
        chartView.data = LineChartData(dataSet: dataSet)
        chartView.rightAxis.enabled = false
        chartView.xAxis.labelPosition = .bottom
        
        view.addSubview(chartView)
        chartView.pin(to: view.safeAreaLayoutGuide)
    }
}
