//
//  UnityBridge.m
//  RudderSDKUnity
//
//  Created by Arnab Pal on 23/10/19.
//  Copyright © 2019 Rudderlabs. All rights reserved.
//

#import "UnityBridge.h"
#import <sqlite3.h>
extern "C"
{
    void _initiateInstance(const char* _writeKey,
                           const char* _endPointUrl,
                           const int _flushQueueSize,
                           const int _dbCountThreshold,
                           const int _sleepTimeout,
                           const int _logLevel) {
        [RudderClientWrapper _initiateInstance:[[NSString alloc] initWithUTF8String:_writeKey]
                                   endPointUrl:[[NSString alloc] initWithUTF8String:_endPointUrl]
                                flushQueueSize:_flushQueueSize
                              dbCountThreshold:_dbCountThreshold
                                  sleepTimeOut:_sleepTimeout
                                      logLevel:_logLevel];
    }

    void _logEvent(const char* _eventType,
                   const char* _eventName,
                   const char* _eventPropsJson,
                   const char* _userPropsJson,
                   const char* _optionsJson) {
        [RudderClientWrapper _logEvent:[[NSString alloc] initWithUTF8String:_eventType]
                             eventName:[[NSString alloc] initWithUTF8String:_eventName]
                        eventPropsJson:[[NSString alloc] initWithUTF8String:_eventPropsJson]
                         userPropsJson:[[NSString alloc] initWithUTF8String:_userPropsJson]
                           optionsJson:[[NSString alloc] initWithUTF8String:_optionsJson]];
    }
    
    void _identify(const char* _userId,
                   const char* _traitsJson,
                   const char* _optionsJson) {
        [RudderClientWrapper _identify:[[NSString alloc] initWithUTF8String:_userId]
                            traitsJson:[[NSString alloc] initWithUTF8String:_traitsJson]
                           optionsJson:[[NSString alloc] initWithUTF8String:_optionsJson]];
    }
    
    void _reset() {
        [RudderClientWrapper _reset];
    }

    void _serializeSqlite() {
        sqlite3_config(SQLITE_CONFIG_SERIALIZED);
    }
}
