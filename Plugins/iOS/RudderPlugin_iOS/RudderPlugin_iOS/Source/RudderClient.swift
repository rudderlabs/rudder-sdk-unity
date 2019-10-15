//
//  RudderClient.swift
//  RudderPlugin_iOS
//
//  Created by Arnab Pal on 14/09/19.
//  Copyright © 2019 Arnab Pal. All rights reserved.
//

import Foundation

class RudderClient: NSObject {
    private static var instance: RudderClient? = nil
    private var repository: EventRepository? = nil
    
    private override init() {
        // do nothing
    }
    
    init(_writeKey: String, _endPointUrl: String, _flushQueueSize: Int32, _dbCountThreshold: Int32, _sleepTimeOut: Int32) {
        self.repository = EventRepository.getInstance(writeKey: _writeKey, endPointUrl: _endPointUrl, flushQueueSize: _flushQueueSize, dbCountThreshold: _dbCountThreshold, sleepTimeOut: _sleepTimeOut)
    }
    
    @objc static func getInstance() -> RudderClient? {
        return instance
    }
    
    @objc static func getInstance(writeKey: String) -> RudderClient {
        return getInstance(writeKey: writeKey, endPointUrl: Constants.BASE_URL)
    }
    
    @objc static func getInstance(writeKey: String, endPointUrl: String) -> RudderClient {
        return getInstance(writeKey: writeKey, endPointUrl: endPointUrl, flushQueueSize: Constants.FLUSH_QUEUE_SIZE)
    }
    
    @objc static func getInstance(writeKey: String, endPointUrl: String, flushQueueSize: Int32) -> RudderClient {
        return getInstance(writeKey: writeKey, endPointUrl: endPointUrl, flushQueueSize: flushQueueSize, dbCountThreshold: Constants.DB_COUNT_THRESHOLD, sleepTimeOut: Constants.SLEEP_TIME_OUT)
    }
    
    @objc static func getInstance(writeKey: String, endPointUrl: String, flushQueueSize: Int32, dbCountThreshold: Int32, sleepTimeOut: Int32) -> RudderClient {
        if (instance == nil) {
            instance = RudderClient(_writeKey: writeKey, _endPointUrl: endPointUrl, _flushQueueSize: flushQueueSize, _dbCountThreshold: dbCountThreshold, _sleepTimeOut: sleepTimeOut)
        }
        return instance!
    }
    
    @objc static func initiateInstance(writeKey: String, endPointUrl: String, flushQueueSize: Int32, dbCountThreshold: Int32, sleepTimeOut: Int32) {
        if (instance == nil) {
            instance = RudderClient(_writeKey: writeKey, _endPointUrl: endPointUrl, _flushQueueSize: flushQueueSize, _dbCountThreshold: dbCountThreshold, _sleepTimeOut: sleepTimeOut)
        }
    }
    
    @objc func track(element: RudderElement) {
        do {
            element.setEventType(type: MessageType.TRACK)
            try repository!.dump(element: element)
        } catch {
            print(error.localizedDescription)
        }
    }
    
    @objc func track(builder: RudderElementBuilder) {
        track(element: builder.build())
    }
    
    @objc func screen(element: RudderElement) {
        do {
            element.setEventType(type: MessageType.SCREEN)
            try repository!.dump(element: element)
        } catch {
            print(error.localizedDescription)
        }
    }
    
    @objc func screen(builder: RudderElementBuilder) {
        screen(element: builder.build())
    }
    
    @objc func page(element: RudderElement) {
        do {
            element.setEventType(type: MessageType.PAGE)
            try repository!.dump(element: element)
        } catch {
            print(error.localizedDescription)
        }
    }
    
    @objc func page(builder: RudderElementBuilder) {
        page(element: builder.build())
    }
    
    @objc func identify(element: RudderElement) {
        do {
            element.setEventType(type: MessageType.IDENTIFY)
            try repository!.dump(element: element)
        } catch {
            print(error.localizedDescription)
        }
    }
    
    @objc func identify(builder: RudderElementBuilder) {
        identify(element: builder.build())
    }
    
    @objc func logEvent(eventType: String, eventName: String, userId: String, eventPropertiesJson: String, userPropertiesJson: String, integrationsJson: String) {
        do {
            let element = RudderElementBuilder()
                .withEventName(eventName: eventName)
                .withUserId(userId: userId)
                .build()
            element.setEventType(type: eventType)
            element.setEventPropertiesString(eventProperties: eventPropertiesJson)
            element.setUserPropertiesString(userProperties: userPropertiesJson)
            element.setIntegrationsString(integrations: integrationsJson)
            try repository?.dump(element: element)
        } catch {
            print(error.localizedDescription)
        }
    }
    
    @objc func logEvent(eventType: String, eventName: String, userId: String, eventProperties: Dictionary<String, NSObject>, userProperties: Dictionary<String, NSObject>, integrations: Dictionary<String, Bool>) {
        do {
            let element = RudderElementBuilder()
                .withEventName(eventName: eventName)
                .withUserId(userId: userId)
                .withEventProperties(properties: eventProperties)
                .withUserProperties(properties: userProperties)
                .withIntegration(integrations: integrations)
                .build()
            element.setEventType(type: eventType)
            try repository?.dump(element: element)
        } catch {
            print(error.localizedDescription)
        }
    }
}
