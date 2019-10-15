    //
//  RudderElementBuilder.swift
//  RudderPlugin_iOS
//
//  Created by Arnab Pal on 14/09/19.
//  Copyright © 2019 Arnab Pal. All rights reserved.
//

import Foundation

class RudderElementBuilder : NSObject {
    private var eventName: String? = nil
    
    @objc func withEventName(eventName: String) -> RudderElementBuilder {
        self.eventName = eventName
        return self
    }
    
    private var userId: String? = nil
    
    @objc func withUserId(userId: String) -> RudderElementBuilder {
        self.userId = userId
        return self
    }
    
    private var eventProperties: Dictionary<String, NSObject>? = nil
    @objc func withEventProperties(properties: Dictionary<String, NSObject>) -> RudderElementBuilder {
        self.eventProperties = properties
        return self
    }
    
    @objc func withEventProperty(key: String, value: NSObject) -> RudderElementBuilder {
        if(self.eventProperties == nil) {
            self.eventProperties = Dictionary()
        }
        self.eventProperties![key] = value
        return self
    }
    
    private var userProperties: Dictionary<String, NSObject>? = nil
    @objc func withUserProperties(properties: Dictionary<String, NSObject>) -> RudderElementBuilder {
        self.userProperties = properties
        return self
    }
    
    @objc func withUserProperty(key: String, value: NSObject) -> RudderElementBuilder {
        if (self.userProperties == nil) {
            self.userProperties = Dictionary()
        }
        self.userProperties![key] = value
        return self
    }
    
    private var integrations: Dictionary<String, Bool>? = nil
    @objc func withIntegration(integrations: Dictionary<String, Bool>) -> RudderElementBuilder {
        self.integrations = integrations
        return self
    }
    
    @objc func withIntegration(integration: String, isEnabled: Bool) -> RudderElementBuilder {
        if (self.integrations == nil) {
            self.integrations = Dictionary()
        }
        self.integrations![integration] = isEnabled
        return self
    }
    
    @objc func build() -> RudderElement {
        let element = RudderElement()
        if (self.eventName != nil) {
            element.setEventName(eventName: self.eventName!)
        }
        if (self.userId != nil) {
            element.setUserId(userId: self.userId!)
        }
        if (self.eventProperties != nil) {
            element.setEventProperties(eventProperties: AnyCodable(value: self.eventProperties!))
        }
        if (self.userProperties != nil) {
            element.setUserProperties(userProperties: AnyCodable(value: self.userProperties!))
        }
        if (self.integrations != nil) {
            element.setIntegrations(integrations: AnyCodable(value: self.integrations!))
        }
        return element
    }
}
