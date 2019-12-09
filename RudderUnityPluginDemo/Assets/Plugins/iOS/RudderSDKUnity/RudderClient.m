//
//  RudderClient.m
//  RudderSDKUnity
//
//  Created by Arnab Pal on 22/10/19.
//  Copyright © 2019 Rudderlabs. All rights reserved.
//

#import "RudderClient.h"
#import "EventRepository.h"
#import "RudderConfig.h"
#import "RudderMessage.h"
#import "RudderMessageBuilder.h"

static RudderClient *_instance = nil;
static EventRepository *_repository = nil;

@implementation RudderClient

+ (void)_initiateInstance:(NSString *)writeKey
              endPointUrl:(NSString *)endPointUrl
           flushQueueSize:(int)flushQueueSize
         dbCountThreshold:(int)dbCountThreshold
             sleepTimeout:(int)sleepTimeout
                 logLevel:(int)logLevel {
    if (_instance == nil) {
        static dispatch_once_t onceToken;
        dispatch_once(&onceToken, ^{
            _instance = [[self alloc] init];
            
            // create RudderConfig based on the provided parameters
            RudderConfig *config = [[RudderConfig alloc] init];
            config.endPointUrl = endPointUrl;
            config.flushQueueSize = flushQueueSize;
            config.dbCountThreshold = dbCountThreshold;
            config.sleepTimeout = sleepTimeout;
            config.logLevel = logLevel;
            
            // initiate repository with the config
            _repository = [EventRepository initiate:writeKey config:config];
        });
    }
}

+ (void) _logEvent:(NSString *)eventType
         eventName:(NSString *)eventName
            userId:(NSString *)userId
eventPropertiesJson:(NSString *)eventPropertiesJson
userPropertiesJson:(NSString *)userPropertiesJson
  integrationsJson:(NSString *)integrationsJson {
    // create error instance
    NSError *error;
    
    // create messageBuilder
    RudderMessageBuilder *builder = [[RudderMessageBuilder alloc] init];
    [builder setEventName:eventName];
    
    // create eventProperties dict from json and set it to builder
    NSDictionary *eventProperties = [NSJSONSerialization JSONObjectWithData:[eventPropertiesJson dataUsingEncoding:NSUTF8StringEncoding] options:NSJSONReadingMutableContainers error:&error];
    [builder setPropertyDict:eventProperties];
    
    // create userProperties dict from json and set it to builder
    NSDictionary *userProperties = [NSJSONSerialization JSONObjectWithData:[userPropertiesJson dataUsingEncoding:NSUTF8StringEncoding] options:NSJSONReadingMutableContainers error:&error];;
    [builder setUserProperty:userProperties];

    // set userId to the builder
    [builder setUserId:userId];
    
    // finally build the message
    RudderMessage *message = [builder build];
    NSDictionary *integrations = [NSJSONSerialization JSONObjectWithData:[integrationsJson dataUsingEncoding:NSUTF8StringEncoding] options:NSJSONReadingMutableContainers error:&error];;
    message.integrations = integrations;
    
    // set the message type after building the message
    message.type = eventType;
    
    // dump the message if repository is initiated
    if (_repository != nil) {
        [_repository dump:message];
    }
}

@end
