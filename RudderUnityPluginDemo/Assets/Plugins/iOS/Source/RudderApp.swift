//
//  RudderApp.swift
//  RudderSample
//
//  Created by Arnab Pal on 12/07/19.
//  Copyright © 2019 Arnab Pal. All rights reserved.
//

import Foundation

struct RudderApp: Encodable {
    var build: String = Bundle.main.infoDictionary?["CFBundleShortVersionString"] as? String ?? ""
    var name: String = Bundle.main.infoDictionary?["CFBundleName"] as? String ?? ""
    var nameSpace : String = Bundle.main.bundleIdentifier ?? ""
    var version: String = Bundle.main.infoDictionary?["CFBundleName"] as? String ?? ""
    
    enum CodingKeys: String, CodingKey {
        case build = "rl_build"
        case name = "rl_name"
        case nameSpace = "rl_namespace"
        case version = "rl_version"
    }
}
