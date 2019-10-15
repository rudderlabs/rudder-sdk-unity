//
//  RudderMessage.swift
//  RudderSample
//
//  Created by Arnab Pal on 10/07/19.
//  Copyright © 2019 Arnab Pal. All rights reserved.
//

import Foundation

struct RudderMessage : Encodable {
    let messageId: String = String(NSDate().timeIntervalSince1970) + "-" + UUID().uuidString.lowercased()
    var channel: String = "ios-sdk-channel"
    var context : RudderContext? = nil
    var type: String = ""
    let action: String = ""
    let timestamp: String = Utils.getTimeStampStr()
    var anonymousId: String = ""
    var userId: String = ""
    var event: String = ""
    var eventProperties: AnyCodable? = nil
    var userProperties: AnyCodable? = nil
    var integrations: AnyCodable? = nil
    
    init() {
        self.context = RudderElementCache.getCachedContext()
        self.anonymousId = self.context!.deviceInfo.id
    }
    
    mutating func setIntegrations(integrations: AnyCodable?) {
        self.integrations = integrations
    }
    
    mutating func setEventProperties(eventProperties: AnyCodable?) {
        self.eventProperties = eventProperties
    }
    
    mutating func setUserProperties(userProperties: AnyCodable?) {
        self.userProperties = userProperties
    }
    
    mutating func setEventType(type: String) {
        self.type = type
    }
    
    mutating func setEventName(eventName: String) {
        self.event = eventName
    }
    
    mutating func setUserId(userId: String) {
        self.userId = userId
    }
    
    enum CodingKeys: String, CodingKey {
        case messageId = "rl_message_id"
        case channel = "rl_channel"
        case context = "rl_context"
        case type = "rl_type"
        case action = "rl_action"
        case timestamp = "rl_timestamp"
        case anonymousId = "rl_anonymous_id"
        case userId = "rl_user_id"
        case event = "rl_event"
        case eventProperties = "rl_properties"
        case userProperties = "rl_user_properties"
        case integrations = "rl_integrations"
    }
    
    func encode(to encoder: Encoder) throws {
        var container = encoder.container(keyedBy: CodingKeys.self)
        try container.encode(messageId, forKey: .messageId)
        try container.encode(channel, forKey: .channel)
        try container.encode(context, forKey: .context)
        try container.encode(type, forKey: .type)
        try container.encode(action, forKey: .action)
        try container.encode(timestamp, forKey: .timestamp)
        try container.encode(anonymousId, forKey: .anonymousId)
        try container.encode(userId, forKey: .userId)
        try container.encode(event, forKey: .event)
        try container.encode(eventProperties, forKey: .eventProperties)
        try container.encode(userProperties, forKey: .userProperties)
        try container.encode(integrations, forKey: .integrations)
    }
}
