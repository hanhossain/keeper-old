//
//  SubtitleRightDetailTableViewCell.swift
//  Keeper
//
//  Created by Han Hossain on 6/24/22.
//

import UIKit

class SubtitleRightDetailTableViewCell: UITableViewCell {
    
    let mainLabel = UILabel()
    let rightLabel = UILabel()
    
    let subtitleLabel: UILabel = {
        let label = UILabel()
        label.font = .preferredFont(forTextStyle: .caption1)
        return label
    }()
    
    override init(style: UITableViewCell.CellStyle, reuseIdentifier: String?) {
        super.init(style: style, reuseIdentifier: reuseIdentifier)
        
        let leftStackView = UIStackView(arrangedSubviews: [mainLabel, subtitleLabel])
        leftStackView.axis = .vertical
        leftStackView.spacing = 8
        
        let mainStackView = UIStackView(arrangedSubviews: [leftStackView, rightLabel])
        mainStackView.layoutMargins = UIEdgeInsets(top: 8, left: 8, bottom: 8, right: 8)
        mainStackView.isLayoutMarginsRelativeArrangement = true
        mainStackView.spacing = 4
        mainStackView.distribution = .equalSpacing
        
        contentView.addSubview(mainStackView)
        mainStackView.pin(to: contentView)
    }
    
    required init?(coder: NSCoder) {
        fatalError("init(coder:) has not been implemented")
    }

    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
    }

    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)

        // Configure the view for the selected state
    }

}
