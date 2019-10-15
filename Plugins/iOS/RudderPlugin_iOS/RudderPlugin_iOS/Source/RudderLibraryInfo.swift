//
//  RudderLibraryInfo.swift
//  RudderSample
//
//  Created by Arnab Pal on 12/07/19.
//  Copyright © 2019 Arnab Pal. All rights reserved.
//

import Foundation

struct RudderLibraryInfo: Encodable {
    let name: String = "rudder-ios-library"
    let version: String = "1.0"
    
    enum CodingKeys: String, CodingKey {
        case name = "rl_name"
        case version = "rl_version"
    }
}
