//
//  RudderElement.swift
//  RudderPlugin_iOS
//
//  Created by Arnab Pal on 14/09/19.
//  Copyright © 2019 Arnab Pal. All rights reserved.
//

import Foundation

class RudderElement: NSObject, Encodable {
    private var message: RudderMessage? = RudderMessage()
    
    func setEventName(eventName: String) {
        message?.setEventName(eventName: eventName)
    }
    
    func setUserId(userId: String) {
        message?.setUserId(userId: userId)
    }
    
    func setEventType(type: String) {
        message?.setEventType(type: type)
    }
    
    func setEventProperties(eventProperties: AnyCodable) {
        message?.setEventProperties(eventProperties: eventProperties)
    }
    
    func setEventPropertiesString(eventProperties: String) {
        let map = self.parseJsonToMap(json: eventProperties)
        message?.setEventProperties(eventProperties: map)
    }
    
    func setUserProperties(userProperties: AnyCodable) {
        message?.setUserProperties(userProperties: userProperties)
    }
    
    func setUserPropertiesString(userProperties: String) {
        let map = self.parseJsonToMap(json: userProperties)
        message?.setUserProperties(userProperties: map)
    }
    
    func setIntegrations(integrations: AnyCodable) {
        message?.setIntegrations(integrations: integrations)
    }
    
    func setIntegrationsString(integrations: String) {
        let map = self.parseJsonToMap(json: integrations)
        message?.setIntegrations(integrations: map)
    }
    
    private func parseJsonToMap(json: String) -> AnyCodable? {
        var data: AnyCodable? = nil
        do {
           data = try JSONDecoder().decode(AnyCodable.self, from: Data(json.utf8))
        } catch {
            // some error occurred. ignore. to prevent crash from try!
        }
        return data
    }
    
    enum CodingKeys: String, CodingKey {
        case message = "rl_message"
    }
}
