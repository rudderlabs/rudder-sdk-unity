//
//  RudderClientWrapper.h
//  Pods-RudderSDKUnityTest
//
//  Created by Arnab Pal on 15/12/19.
//

#import <Foundation/Foundation.h>
#import "RSLogger.h"

NS_ASSUME_NONNULL_BEGIN

@interface RudderClientWrapper : NSObject

+ (void) _initiateInstance: (NSString*) _anonymousId
                  writeKey: (NSString*) _writeKey
              dataPlaneUrl: (NSString*) _dataPlaneUrl
           controlPlaneUrl: (NSString*) _controlPlaneUrl
            flushQueueSize: (int) _flushQueueSize
          dbCountThreshold: (int) _dbCountThreshold
              sleepTimeOut: (int) _sleepTimeout
                configRefreshInterval: (int) _configRefreshInterval
      trackLifecycleEvents: (BOOL) _trackLifecycleEvents
         recordScreenViews: (BOOL) _recordScreenViews
                  logLevel: (int) _logLevel;

+ (void) _logEvent: (NSString*) _eventType
         eventName: (NSString*) _eventName
    eventPropsJson: (NSString*) _eventPropsJson
     userPropsJson: (NSString*) _userPropsJson
       optionsJson: (NSString*) _optionsJson;

+ (void) _identify: (NSString*) _userId
        traitsJson: (NSString*) _traitsJson
       optionsJson: (NSString*) _optionsJson;

+ (void) _reset;

+ (void) _setAnonymousId: (NSString*) _anonymousId;

@end

NS_ASSUME_NONNULL_END
