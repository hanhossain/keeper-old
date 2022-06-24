//
//  SubtitleRightDetailTableViewCell.swift
//  Keeper
//
//  Created by Han Hossain on 6/24/22.
//

import UIKit

class SubtitleRightDetailTableViewCell: UITableViewCell {

    @IBOutlet weak var mainText: UILabel!
    @IBOutlet weak var subtitle: UILabel!
    @IBOutlet weak var rightDetail: UILabel!

    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
    }

    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)

        // Configure the view for the selected state
    }

}
