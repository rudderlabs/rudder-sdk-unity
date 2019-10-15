//
//  TraitsAddress.swift
//  RudderSample
//
//  Created by Arnab Pal on 23/07/19.
//  Copyright © 2019 Arnab Pal. All rights reserved.
//

import Foundation

struct TraitsAddress : Encodable {
    var city: String? = nil
    var country: String? = nil
    var postalCode: String? = nil
    var state: String? = nil
    var street: String? = nil
    
    enum CodingKeys: String, CodingKey {
        case city = "rl_city"
        case country = "rl_country"
        case postalCode = "rl_postalcode"
        case state = "rl_state"
        case street = "rl_street"
    }
}
