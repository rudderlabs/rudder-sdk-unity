//
//  RudderNetwork.swift
//  RudderSample
//
//  Created by Arnab Pal on 12/07/19.
//  Copyright © 2019 Arnab Pal. All rights reserved.
//

import Foundation
import CoreTelephony

struct RudderNetwork: Encodable {
    var carrier: String = CTCarrier().carrierName ?? "unavailable"
    
    enum CodingKeys: String, CodingKey {
        case carrier = "rl_carrier"
    }
}
