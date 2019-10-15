//
//  RudderClient-Bridge.m
//  RudderPlugin_iOS
//
//  Created by Arnab Pal on 14/09/19.
//  Copyright © 2019 Arnab Pal. All rights reserved.
//

#import "RudderPlugin_iOS-Swift.h"

#pragma mark - C interface

extern "C" {
    void _initiateInstance(char* writeKey, char* endPointUrl, int flushQueueSize, int dbCountThreshold, int sleepTimeOut) {
        [RudderClient
         initiateInstanceWithWriteKey:[NSString stringWithUTF8String:writeKey]
         endPointUrl:[NSString stringWithUTF8String:endPointUrl]
         flushQueueSize:flushQueueSize
         dbCountThreshold:dbCountThreshold
         sleepTimeOut:sleepTimeOut];
    }
    
    void _logEvent(char* eventType, char* eventName, char* userId, char* eventPropertiesJson, char* userPropertiesJson, char* integrationsJSon) {
//        NSLog(@"eventName: %s", integrationsJSon);
        [[RudderClient getInstance]
         logEventWithEventType:[NSString stringWithUTF8String:eventType]
         eventName:[NSString stringWithUTF8String:eventName]
         userId:[NSString stringWithUTF8String:userId]
         eventPropertiesJson:[NSString stringWithUTF8String:eventPropertiesJson]
         userPropertiesJson:[NSString stringWithUTF8String:userPropertiesJson]
         integrationsJson:[NSString stringWithUTF8String:integrationsJSon]];
    }
}
