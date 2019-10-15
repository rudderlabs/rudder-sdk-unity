//
//  RudderContext.swift
//  RudderSample
//
//  Created by Arnab Pal on 12/07/19.
//  Copyright © 2019 Arnab Pal. All rights reserved.
//

import Foundation

struct RudderContext: Encodable {
    var app: RudderApp = RudderApp()
    var traits: RudderTraits? = nil
    var library: RudderLibraryInfo = RudderLibraryInfo()
    var os: RudderOSInfo = RudderOSInfo()
    var screenInfo: RudderScreenInfo = RudderScreenInfo()
    var userAgent: String = "rudder-ios-client"
    var locale: String = (Locale.current.languageCode ?? "") + "-" + (Locale.current.regionCode ?? "")
    var deviceInfo: RudderDeviceInfo = RudderDeviceInfo()
    var network: RudderNetwork = RudderNetwork()
    var platform: String = "ios"
    
    init() {
        self.traits = RudderTraits(anonymousId: self.deviceInfo.id)
    }
    
    enum CodingKeys: String, CodingKey {
        case app = "rl_app"
        case traits = "rl_traits"
        case library = "rl_library"
        case os = "rl_os"
        case screenInfo = "rl_screen"
        case userAgent = "rl_user_agent"
        case locale = "rl_locale"
        case deviceInfo = "rl_device"
        case network = "rl_network"
        case platform = "rl_platform"
    }
}
