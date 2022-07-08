//
//  UIView+Extensions.swift
//  Keeper
//
//  Created by Han Hossain on 7/1/22.
//

import Foundation
import UIKit

extension UIView {
    func pin(to destination: UIView) {
        self.translatesAutoresizingMaskIntoConstraints = false
        
        NSLayoutConstraint.activate([
            self.topAnchor.constraint(equalTo: destination.topAnchor),
            self.leftAnchor.constraint(equalTo: destination.leftAnchor),
            self.rightAnchor.constraint(equalTo: destination.rightAnchor),
            self.bottomAnchor.constraint(equalTo: destination.bottomAnchor)
        ])
    }
    
    func pin(to destination: UILayoutGuide) {
        self.translatesAutoresizingMaskIntoConstraints = false
        
        NSLayoutConstraint.activate([
            self.topAnchor.constraint(equalTo: destination.topAnchor),
            self.leftAnchor.constraint(equalTo: destination.leftAnchor),
            self.rightAnchor.constraint(equalTo: destination.rightAnchor),
            self.bottomAnchor.constraint(equalTo: destination.bottomAnchor)
        ])
    }
}
