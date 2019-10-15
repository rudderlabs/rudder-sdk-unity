//
//  TraitsCompany.swift
//  RudderSample
//
//  Created by Arnab Pal on 23/07/19.
//  Copyright © 2019 Arnab Pal. All rights reserved.
//

import Foundation

struct TraitsCompany: Encodable {
    var name: String? = nil
    var id: String? = nil
    var industry: String? = nil
    
    enum CodingKeys: String, CodingKey {
        case name = "rl_name"
        case id = "rl_id"
        case industry = "rl_industry"
    }
}
